using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Flurl;

namespace TrustyPay.Core.Cryptography.Http.Service
{
    public class VerifySignatureMiddleware
    {
        private readonly IResourceProvider _provider;

        private readonly ILogger<VerifySignatureMiddleware> _logger;

        private readonly RequestDelegate _next;

        private Header _header;

        public VerifySignatureMiddleware(
            RequestDelegate next,
            IResourceProvider provider,
            ILogger<VerifySignatureMiddleware> logger
        )
        {
            _next = next;
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context)
        {
            if (!IsSignatureVerificationRequired(context))
            {
                await _next.Invoke(context);
                return;
            }

            Dictionary<string, JToken> requestBody;
            try
            {
                requestBody = await ReadRequestBodyAsync(context.Request);
            }
            catch (NotSupportedException se)
            {
                _logger.LogError("ReadRequestBodyAsync " + se.Message);
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status400BadRequest,
                    $"{se.Message},目前只支持application/json和application/x-www-form-urlencoded!");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReadRequestBodyAsync 解析请求参数失败!");
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status400BadRequest,
                    "解析请求参数失败!");
                return;
            }

            try
            {
                EnsureAppIdAndApiKey(requestBody);
            }
            catch (MissingAppIdOrApiKeyException)
            {
                _logger.LogWarning("缺少app id和api key信息!");
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status400BadRequest,
                    "缺少app id和api key信息!");
                return;
            }

            try
            {
                EnsureSignature(requestBody);
            }
            catch (MissingSignatureException)
            {
                _logger.LogWarning("缺少请求签名信息!");
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status400BadRequest,
                    "缺少请求签名信息!");
                return;
            }

            RSACryptoProvider.PublicKey appPublicKey;
            try
            {
                appPublicKey = await _provider.GetAppPublicKeyAsync(
                    requestBody["appId"].ToString(),
                    requestBody["apiKey"].ToString());
            }
            catch (Exception ex5)
            {
                _logger.LogError(ex5, "GetAppPublicKeyAsync Error");
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status401Unauthorized,
                    "获取应用方公钥信息失败!");
                return;
            }

            try
            {
                ServiceSigner.VerifyRequestBody(
                    appPublicKey, context.Request.Path, requestBody);
            }
            catch (InvalidPublicKeyFormatException)
            {
                _logger.LogError("VerifyRequestBody 应用方公钥无效!");
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status401Unauthorized,
                    "应用方公钥无效!");
                return;
            }
            catch (InvalidSignatureException)
            {
                _logger.LogError("VerifyRequestBody 请求签名验证无效!");
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status401Unauthorized,
                    "请求签名验证无效!");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "VerifyRequestBody Error");
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status401Unauthorized,
                    "请求签名验证无效, 请确认参数是否有效!");
                return;
            }

            try
            {
                if (!await _provider.HasPermissionByApiKeyAsync(
                    requestBody["apiKey"].ToString(),
                    context.Request.Path,
                    context.Request.Method))
                {
                    _logger.LogWarning($"{requestBody["apiKey"]} 应用权限不够=>{context.Request.Method}{context.Request.Path}");
                    await WriteResponseErrorAsync(context.Response,
                        context.Request.Path, StatusCodes.Status403Forbidden,
                        "应用权限不够!");
                    return;
                }
            }
            catch (Exception ex7)
            {
                _logger.LogError(ex7, "HasPermissionByApiKeyAsync 权限验证失败!");
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status403Forbidden,
                    "权限验证失败!");
                return;
            }

            try
            {
                InjectBizContent(context.Request, requestBody);
            }
            catch (NotSupportedException se)
            {
                _logger.LogError("InjectBizContent " + se.Message);
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status400BadRequest,
                    se.Message);
                return;
            }
            catch (ArgumentException ae)
            {
                _logger.LogError("InjectBizContent " + ae.Message);
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status400BadRequest,
                    ae.Message);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "InjectBizContent 解析bizContent失败!");
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status400BadRequest,
                    "解析bizContent失败!");
                return;
            }

            try
            {
                await WriteResponseBodyAsync(context.Response, context.Request.Path,
                    async ctx =>
                    {
                        await _next.Invoke(ctx);
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WriteResponseBodyAsync 响应请求失败!");
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status500InternalServerError,
                    "响应请求失败!");
                return;
            }
        }

        private static bool IsSignatureVerificationRequired(HttpContext context)
        {
            var attribute = context.Features?
                .Get<IEndpointFeature>()?
                .Endpoint?
                .Metadata.GetMetadata<SignatureVerificationAttribute>();
            if (attribute == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Read request body
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Dictionary&lt;string, JToken&gt;</returns>
        private async Task<Dictionary<string, JToken>> ReadRequestBodyAsync(HttpRequest request)
        {
            var content = new Dictionary<string, JToken>(10);
            // Parse query parameters
            if (request.Query.Count > 0)
            {
                foreach (var item in request.Query)
                {
                    content.Add(item.Key, item.Value.ToString());
                }
            }

            // Parse request body
            _header = ParseContentType(request.Headers);
            if (request.Body.CanSeek)
            {
                request.Body.Seek(0, SeekOrigin.Begin);
            }
            using var reader = new StreamReader(request.Body, _header.ToEncoding(), true);

            _ = ParseRequestBody(_header.MediaType, await reader.ReadToEndAsync())
                .Aggregate(content, (acc, kv) =>
                {
                    acc.Add(kv.Key, kv.Value);
                    return acc;
                });
            return content;
        }

        private async Task WriteResponseBodyAsync(HttpResponse response, string url,
            Func<HttpContext, Task> handler)
        {
            var original = response.Body;
            response.Body = new MemoryStream();

            await handler(response.HttpContext);

            response.Headers.Remove("Content-Length");
            if (response.Body.CanSeek)
            {
                response.Body.Seek(0, SeekOrigin.Begin); // move to begin
            }

            response.StatusCode = StatusCodes.Status200OK;
            response.ContentType = "application/json;charset=utf-8";

            using var reader = new StreamReader(response.Body);
            var responseBody = ServiceSigner.SignResponseBody(
                _provider.GetPlatformPrivateKey(),
                url,
                JToken.Parse(await reader.ReadToEndAsync()));

            var responseBodyWrapped = JsonConvert.SerializeObject(responseBody, Formatting.None).FromUTF8String();
            response.Headers.ContentLength = responseBodyWrapped == null ? 0 : responseBodyWrapped.Length;
            await original.WriteAsync(responseBodyWrapped);

            response.Body = original;
            await response.CompleteAsync();
        }

        private async Task WriteResponseErrorAsync(
            HttpResponse response, string url, int statusCode, string error)
        {
            response.StatusCode = statusCode;
            response.ContentType = "application/json;charset=utf-8";
            try
            {
                var responseBody = ServiceSigner.SignResponseBody(
                    _provider.GetPlatformPrivateKey(),
                    url,
                    JToken.FromObject(new { code = statusCode, message = error }));

                await response.WriteAsync(
                    JsonConvert.SerializeObject(responseBody, Formatting.None));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WriteResponseErrorAsync Error");
                return;
            }
            finally
            {
                await response.CompleteAsync();
            }
        }

        private static void EnsureAppIdAndApiKey(Dictionary<string, JToken> body)
        {
            if (!body.ContainsKey("appId")
            || !body.ContainsKey("apiKey"))
            {
                throw new MissingAppIdOrApiKeyException();
            }
        }

        private static void EnsureSignature(Dictionary<string, JToken> body)
        {
            if (!body.ContainsKey("sign")
            || string.IsNullOrWhiteSpace(body["sign"].Value<string>()))
            {
                throw new MissingSignatureException();
            }
        }

        /// <summary>
        /// 解析Content-Type
        /// </summary>
        /// <param name="headers">http request headers</param>
        /// <returns>Item1:media type; Item2:charset</returns>
        private static Header ParseContentType(IHeaderDictionary headers)
        {
            string mediaType = "application/json", charset = "utf-8";
            if (headers.TryGetValue("Content-Type", out StringValues value))
            {
                if (!StringValues.IsNullOrEmpty(value))
                {
                    var sections = value.ToString().Replace(" ", "").Split(';');
                    if (sections.Length > 1)
                    {
                        if (sections[1] != null)
                        {
                            var part = sections[1].ToLower();
                            if (part.StartsWith("charset="))
                            {
                                charset = part["charset=".Length..];
                            }
                        }
                    }

                    if (sections.Length > 0)
                    {
                        if (sections[0] != null)
                        {
                            mediaType = sections[0].ToLower();
                        }
                    }
                }
            }
            return new Header(mediaType, charset);
        }

        /// <summary>
        /// Parse request body by media type
        /// </summary>
        /// <param name="mediaType">media type, only support application/json and application/x-www-form-urlencoded</param>
        /// <param name="body">request body</param>
        /// <returns>Dictionary</returns>
        /// <exception cref="NotSupportedException"></exception>
        private static Dictionary<string, JToken> ParseRequestBody(string mediaType, string body)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                // When the request method is get or delete, its body is null.
                return new Dictionary<string, JToken>();
            }

            return mediaType switch
            {
                "application/json" => JsonConvert.DeserializeObject<Dictionary<string, JToken>>(body),
                "application/x-www-form-urlencoded" => ParseForm(body),
                _ => throw new NotSupportedException($"The media type[{mediaType}] is invalid!!!"),
            };
        }

        /// <summary>
        /// Parse form data from body
        /// </summary>
        /// <param name="body">request body</param>
        /// <returns>Dictionary</returns>
        private static Dictionary<string, JToken> ParseForm(string body)
        {
            var content = new Dictionary<string, JToken>(8);
            if (string.IsNullOrEmpty(body))
            {
                return content;
            }
            // 
            // reference https://url.spec.whatwg.org/#application/x-www-form-urlencoded
            //
            var parts = body.Split('&');
            foreach (var part in parts)
            {
                if (part == "" || part.Trim() == "")
                {
                    continue;
                }

                var item = part.Trim();
                var equalIndex = item.IndexOf('=', 0);
                if (equalIndex == -1)
                {
                    content.Add(Url.Decode(item, true), string.Empty);
                }
                else
                {
                    content.Add(
                        Url.Decode(item[..equalIndex], true),
                        Url.Decode(item[(equalIndex + 1)..], true));
                }
            }
            return content;
        }

        /// <summary>
        /// Inject biz content into web api
        /// </summary>
        /// <param name="request"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        private void InjectBizContent(HttpRequest request, Dictionary<string, JToken> body)
        {
            if (!body.ContainsKey("bizContent"))
            {
                throw new ArgumentException("缺少bizContent数据!");
            }

            var value = body["bizContent"];
            switch (request.Method)
            {
                case "GET":
                case "DELETE":
                    request.Query = ConvertBizContentToQueries(value);
                    break;
                case "POST":
                case "PUT":
                    request.Body = ConvertBizContentToJson(_header.MediaType, value);
                    break;
                default:
                    throw new NotSupportedException($"不支持的http方法'{request.Method}'!");
            }
        }

        private QueryCollection ConvertBizContentToQueries(JToken bizContent)
        {
            var queries = bizContent.Type == JTokenType.Object || bizContent.Type == JTokenType.Array
                ? JsonConvert.DeserializeObject<Dictionary<string, StringValues>>(bizContent.ToString(), new StringValuesConverter()) // object/array
                : new Dictionary<string, StringValues>() { { "bizContent", bizContent.ToString() } };                                 // primitive type
            return new QueryCollection(queries);
        }

        private Stream ConvertBizContentToJson(string mediaType, JToken bizContent)
        {
            string buffer;
            if (bizContent.Type == JTokenType.Object || bizContent.Type == JTokenType.Array)
            {
                buffer = mediaType switch
                {
                    "application/json" => bizContent.ToString(),
                    "application/x-www-form-urlencoded" => ConvertBizContentToForm(bizContent),
                    _ => string.Empty
                };
            }
            else
            {
                buffer = mediaType switch
                {
                    "application/json" => bizContent.ToString(),
                    "application/x-www-form-urlencoded" => Url.Encode(bizContent.ToString(), true),
                    _ => string.Empty
                };
            }
            return new MemoryStream(buffer.FromUTF8String());
        }

        private string ConvertBizContentToForm(JToken bizContent)
        {
            var value = bizContent.ToString();
            var form = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(value);
            var buffer = form.Aggregate(new StringBuilder(value.Length), (acc, kv) =>
            {
                acc.Append(kv.Key + "=" + Url.Encode(kv.Value.ToString(), true) + "&");
                return acc;
            });
            return buffer.ToString(0, buffer.Length - 1);
        }

        /// <summary>
        /// A request header struct
        /// </summary>
        private struct Header
        {
            public Header(string mediaType, string charset)
            {
                MediaType = mediaType;
                Charset = charset;
            }

            public string MediaType { get; private set; }

            public string Charset { get; private set; }

            public Encoding ToEncoding()
            {
                Encoding encoding;
                if (Charset.Equals("utf-8", StringComparison.CurrentCultureIgnoreCase))
                {
                    encoding = Encoding.UTF8;
                }
                else if (Charset.Equals("ascii", StringComparison.CurrentCultureIgnoreCase))
                {
                    encoding = Encoding.ASCII;
                }
                else
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    encoding = Encoding.GetEncoding(Charset);
                }
                return encoding;
            }
        }
    }
}
