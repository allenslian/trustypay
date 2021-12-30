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
            }

            Dictionary<string, JToken> requestBody;
            try
            {
                requestBody = await ReadRequestBodyAsync(context.Request);
            }
            catch (NotSupportedException ex1)
            {
                _logger.LogError(ex1, "ReadRequestBodyAsync Error");
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status400BadRequest,
                    "不支持的Media Type,目前只支持application/json和application/x-www-form-urlencoded!");
                return;
            }
            catch (Exception ex2)
            {
                _logger.LogError(ex2, "ReadRequestBodyAsync Error");
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
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status400BadRequest,
                    "缺少app id和api key信息!");
                return;
            }

            RSACryptoProvider.PublicKey appPublicKey;
            try
            {
                appPublicKey = await _provider.GetAppPublicKeyAsync(
                    requestBody["appId"].ToString(),
                    requestBody["apiKey"].ToString());

                EnsureAppPublicKey(appPublicKey);
            }
            catch (InvalidAppPublicKeyException ex4)
            {
                _logger.LogError(ex4, "GetAppPublicKeyAsync Error");
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status401Unauthorized,
                    "应用方公钥无效!");
                return;
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
            catch (InvalidSignatureException ex6)
            {
                _logger.LogError(ex6, "VerifyRequestBody Error");
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status401Unauthorized,
                    "请求签名验证无效!");
                return;
            }

            try
            {
                if (!await _provider.HasPermissionByApiKeyAsync(
                    requestBody["apiKey"].ToString(),
                    context.Request.Path,
                    context.Request.Method))
                {
                    _logger.LogWarning($"{requestBody["apiKey"]} => {context.Request.Method}{context.Request.Path}");
                    await WriteResponseErrorAsync(context.Response,
                        context.Request.Path, StatusCodes.Status403Forbidden,
                        "应用权限不够!");
                    return;
                }
            }
            catch (Exception ex7)
            {
                _logger.LogError(ex7, "HasPermissionByApiKeyAsync Error");
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status403Forbidden,
                    "权限验证失败!");
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
            catch (Exception ex8)
            {
                _logger.LogError(ex8, "WriteResponseBodyAsync Error");
                await WriteResponseErrorAsync(context.Response,
                    context.Request.Path, StatusCodes.Status500InternalServerError,
                    "响应请求失败!");
                return;
            }
        }

        private static bool IsSignatureVerificationRequired(HttpContext context)
        {
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            if (endpoint == null)
            {
                return false;
            }

            var attribute = endpoint?.Metadata.GetMetadata<SignatureVerificationAttribute>();
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
        private static async Task<Dictionary<string, JToken>> ReadRequestBodyAsync(HttpRequest request)
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
            var header = ParseContentType(request.Headers);
            using var reader = new StreamReader(request.Body, header.ToEncoding(), true);

            _ = ParseRequestBody(header.MediaType, await reader.ReadToEndAsync())
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
            try
            {
                response.Body.Seek(0, SeekOrigin.Begin); // move to begin
                using var reader = new StreamReader(response.Body);
                var responseBody = ServiceSigner.SignResponseBody(
                    _provider.GetPlatformPrivateKey(),
                    url, JToken.Parse(await reader.ReadToEndAsync()));

                var buffer = JsonConvert.SerializeObject(responseBody, Formatting.None);
                await original.WriteAsync(Encoding.UTF8.GetBytes(buffer));
            }
            finally
            {
                response.Body = original;
            }
        }

        private async Task WriteResponseErrorAsync(
            HttpResponse response, string url, int statusCode, string error)
        {
            try
            {
                response.StatusCode = statusCode;
                response.ContentType = "application/json;charset=utf-8";

                var responseBody = ServiceSigner.SignResponseBody(
                    _provider.GetPlatformPrivateKey(),
                    url,
                    new { code = statusCode, message = error });
                await response.WriteAsync(responseBody.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WriteResponseErrorAsync Error");
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

        private static void EnsureAppPublicKey(RSACryptoProvider.PublicKey publicKey)
        {
            if (publicKey == null
            || publicKey.Key == null
            || publicKey.Key.Length < 1024)
            {
                throw new InvalidAppPublicKeyException();
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
            foreach (var item in parts)
            {
                if (item == "" || item.Trim() == "")
                {
                    continue;
                }

                var equalIndex = item.IndexOf('=', 0, 1);
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
                if (Charset == "utf-8")
                {
                    encoding = Encoding.UTF8;
                }
                else if (Charset == "ascii")
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
