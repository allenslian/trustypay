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
    public class SignatureMiddleware
    {
        private readonly IResourceProvider _provider;

        private readonly ILogger<SignatureMiddleware> _logger;

        private readonly RequestDelegate _next;

        private readonly SignatureOption _option;

        private ContentType _header;

        public SignatureMiddleware(
            RequestDelegate next,
            SignatureOption option,
            IResourceProvider provider,
            ILogger<SignatureMiddleware> logger
        )
        {
            _next = next;
            _option = option;
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
                EnsureSignature(requestBody);
                EnsureTimestamp(requestBody);
            }
            catch (MissingAppIdOrApiKeyException)
            {
                _logger.LogWarning("缺少app id和api key信息!");
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status400BadRequest,
                    "缺少app id和api key信息!");
                return;
            }
            catch (MissingSignatureException)
            {
                _logger.LogWarning("缺少请求签名信息!");
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status400BadRequest,
                    "缺少请求签名信息!");
                return;
            }
            catch (InvalidTimestampException)
            {
                _logger.LogWarning("无效的时间戳!");
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status400BadRequest,
                    "无效的时间戳!");
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
                InjectBizContent(context, requestBody);
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

            return ParseRequestBody(_header.MediaType, await reader.ReadToEndAsync())
                .Aggregate(content, (acc, kv) =>
                {
                    acc[kv.Key] = kv.Value;
                    return acc;
                });
        }

        private async Task WriteResponseBodyAsync(HttpResponse response, string url,
            Func<HttpContext, Task> handler)
        {
            var original = response.Body;
            response.Body = new MemoryStream();

            await handler(response.HttpContext);

            response.StatusCode = StatusCodes.Status200OK;
            response.ContentType = "application/json;charset=utf-8";

            response.Body.Seek(0, SeekOrigin.Begin); // move to begin
            using var reader = new StreamReader(response.Body);
            var responseBody = ServiceSigner.SignResponseBody(
                _provider.GetPlatformPrivateKey(),
                url,
                JToken.Parse(await reader.ReadToEndAsync()));

            if (original.CanWrite)
            {
                var responseBodyWrapped = JsonConvert.SerializeObject(responseBody, Formatting.None).FromUTF8String();

                response.Headers.Remove("Content-Length");
                response.Headers.ContentLength = responseBodyWrapped == null ? 0 : responseBodyWrapped.Length;
                await original.WriteAsync(responseBodyWrapped);
            }
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

        private void EnsureTimestamp(Dictionary<string, JToken> body)
        {
            if (_option.ValidateTimestamp)
            {
                if (!body.ContainsKey("timestamp"))
                {
                    throw new InvalidTimestampException();
                }

                DateTime timestamp;
                try
                {
                    var value = body["timestamp"].Value<long>();
                    timestamp = DateTime.UnixEpoch.Add(TimeSpan.FromMilliseconds(value));
                }
                catch (Exception te)
                {
                    _logger.LogError(te, $"timestamp[{body["timestamp"]}] is invalid!!!");
                    throw new InvalidTimestampException();
                }

                if (timestamp < DateTime.UtcNow.AddMinutes(-10) || timestamp > DateTime.UtcNow.AddMinutes(10))
                {
                    _logger.LogWarning($"timestamp[{timestamp}] is invalid!!!");
                    throw new InvalidTimestampException();
                }
            }
        }

        /// <summary>
        /// Parse Content-Type
        /// </summary>
        /// <param name="headers">http request headers</param>
        /// <returns>Header object</returns>
        private static ContentType ParseContentType(IHeaderDictionary headers)
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
            return new ContentType(mediaType, charset);
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
        /// <param name="context"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        private void InjectBizContent(HttpContext context, Dictionary<string, JToken> body)
        {
            if (!body.ContainsKey("bizContent"))
            {
                throw new ArgumentException("缺少bizContent数据!");
            }

            // inject app id into context
            context.Items[_option.AppIdName] = body["appId"].Value<string>();
            var value = body["bizContent"];
            switch (context.Request.Method)
            {
                case "GET":
                case "DELETE":
                    context.Request.Query = ConvertBizContentToQueries(value);
                    break;
                case "POST":
                case "PUT":
                    context.Request.Body = ConvertBizContentToJson(_header.MediaType, value);
                    break;
                default:
                    throw new NotSupportedException($"不支持的http方法'{context.Request.Method}'!");
            }
        }

        private QueryCollection ConvertBizContentToQueries(JToken bizContent)
        {
            if (bizContent == null)
            {
                return new QueryCollection();
            }

            var value = bizContent.ToString(Formatting.None);
            if (value.EndsWith('}'))
            {
                var queries = JsonConvert.DeserializeObject<Dictionary<string, StringValues>>(bizContent.ToString(), new StringValuesConverter());
                return new QueryCollection(queries);
            }
            else
            {
                var queries = JsonConvert.DeserializeObject<StringValues>(bizContent.ToString(), new StringValuesConverter());
                return new QueryCollection(new Dictionary<string, StringValues>() { { "bizContent", queries } });
            }
        }

        private Stream ConvertBizContentToJson(string mediaType, JToken bizContent)
        {
            string buffer = mediaType switch
            {
                "application/json" => bizContent.ToString(),
                "application/x-www-form-urlencoded" => ConvertBizContentToForm(bizContent),
                _ => string.Empty
            };
            return new MemoryStream(buffer.FromUTF8String());
        }

        private string ConvertBizContentToForm(JToken bizContent)
        {
            if (bizContent == null)
            {
                return string.Empty;
            }

            var value = bizContent.ToString(Formatting.None);
            if (value.EndsWith('}'))
            {
                var form = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(value);
                var buffer = form.Aggregate(new StringBuilder(value.Length), (acc, kv) =>
                {
                    acc.Append(kv.Key + "=" + Url.Encode(kv.Value.ToString(Formatting.None), true) + "&");
                    return acc;
                });
                return buffer.ToString(0, buffer.Length - 1);
            }
            else
            {
                var form = JsonConvert.DeserializeObject<JToken>(value);
                return "bizContent=" + Url.Encode(form.ToString(Formatting.None), true);
            }
        }

        /// <summary>
        /// A request header struct
        /// </summary>
        private struct ContentType
        {
            public ContentType(string mediaType, string charset)
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
