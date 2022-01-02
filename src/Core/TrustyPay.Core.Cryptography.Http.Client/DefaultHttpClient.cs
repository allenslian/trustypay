using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using TrustyPay.Core.Cryptography.Http;

namespace TrustyPay.Core.Cryptography.Http.Client
{
    /// <summary>
    /// A http client for communicating with TrustyPay web api.
    /// </summary>
    public sealed class DefaultHttpClient : HttpClientBase
    {
        public enum SignType
        {
            RS256 = 1,

            RS384 = 2,

            RS512 = 3
        }

        /// <summary>
        /// key constants
        /// </summary>
        public class Constants
        {
            public const string AppId = "appId";

            public const string AppKey = "apiKey";

            public const string BizContent = "bizContent";

            public const string Charset = "charset";

            public const string SignType = "signType";

            public const string Sign = "sign";

            public const string Timestamp = "timestamp";

            public const string Result = "result";
        }

        /// <summary>
        /// App id
        /// </summary>
        private readonly string _appId;

        /// <summary>
        /// Api key
        /// </summary>
        private readonly string _apiKey;

        /// <summary>
        /// Signature type
        /// </summary>
        private readonly SignType _signType;

        /// <summary>
        /// Default charset
        /// </summary>
        private readonly string _charset;

        /// <summary>
        /// Is there a timestamp in the signature?
        /// </summary>
        private readonly bool _hasTimestamp;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="apiBaseUrl">api base url may be empty</param>
        /// <param name="appId">app id</param>
        /// <param name="apiKey">api key</param>
        /// <param name="signType">signature type</param>
        /// <param name="charset">default charset</param>
        /// <param name="hasTimestamp"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DefaultHttpClient(string apiBaseUrl, string appId, string apiKey,
            SignType signType = SignType.RS256, string charset = "utf-8", bool hasTimestamp = true) : base(apiBaseUrl)
        {
            _appId = appId ?? throw new ArgumentNullException(nameof(appId));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _signType = signType;
            _charset = charset;
            _hasTimestamp = hasTimestamp;
        }

        protected override U GetResponseBizContent<U>(IReadOnlyDictionary<string, object> body)
        {
            if (!body.ContainsKey(Constants.Result))
            {
                throw new ArgumentException("No any result!!!", nameof(body));
            }

            var value = body[Constants.Result];
            if (value == null)
            {
                return default;
            }

            if (IsPrimitiveType<U>())
            {
                return (U)value;
            }

            return JsonConvert.DeserializeObject<U>(value.ToString());
        }

        protected override Exception GetResponseError(IReadOnlyDictionary<string, object> body)
        {
            if (!body.ContainsKey(Constants.Result))
            {
                throw new ArgumentException("No any result in the response body!!!");
            }

            var value = body[Constants.Result];
            if (value == null)
            {
                return new ArgumentException("The error in the response body is null!");
            }

            ResponseError error;
            try
            {
                error = JsonConvert.DeserializeObject<ResponseError>(value.ToString());
            }
            catch (JsonSerializationException ex)
            {
                throw new ArgumentException("The result doesn't contain any property called 'message'!", ex);
            }

            return new Exception(error.Message);
        }

        protected override Dictionary<string, object> InitializeRequestBody<T>(T bizContent, IReadOnlyDictionary<string, object> extra = null)
        {
            var body = new Dictionary<string, object>
            {
                {Constants.AppId, _appId},
                {Constants.AppKey, _apiKey},
                {Constants.Charset, _charset},
                {Constants.BizContent, IsPrimitiveType<T>() ? bizContent : JsonConvert.SerializeObject(bizContent, Formatting.None)},
            };

            if (_hasTimestamp)
            {
                body.Add(Constants.Timestamp, DateTimeOffset.Now.ToUnixTimeMilliseconds());
            }

            if (Signer != null)
            {
                body.Add(Constants.SignType, _signType);
            }

            if (extra != null)
            {
                foreach (var kv in extra)
                {
                    body[kv.Key] = kv.Value;
                }
            }
            return body;
        }

        protected override Dictionary<string, object> LoadResponseBody(string content)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
        }

        protected override void Sign(string url, ref Dictionary<string, object> body)
        {
            var buffer = new StringBuilder(url);
            if (!url.Contains('?'))
            {
                buffer.Append('?');
            }
            foreach (var k in body.Keys.OrderBy(m => m))
            {
                var v = body[k];
                if (string.IsNullOrEmpty(k)
                    || v == null
                    || k == Constants.Sign)
                {
                    continue;
                }
                buffer.Append($"{k}={v}&");
            }

            var plainBytes = buffer.ToString(0, buffer.Length - 1).FromCharsetString(
                body.ContainsKey(Constants.Charset) ? body[Constants.Charset].ToString() : _charset);
            body[Constants.Sign] = Signer.Sign(plainBytes, GetHashAlgorithmName(body)).ToBase64String();
        }

        protected override bool Verify(string url, IReadOnlyDictionary<string, object> body)
        {
            if (!body.ContainsKey(Constants.Sign))
            {
                throw new MissingSignatureException();
            }

            var buffer = new StringBuilder(url);
            if (!url.Contains('?'))
            {
                buffer.Append('?');
            }
            foreach (var k in body.Keys.OrderBy(m => m))
            {
                var v = body[k];
                if (string.IsNullOrEmpty(k)
                    || v == null
                    || k == Constants.Sign)
                {
                    continue;
                }
                buffer.Append($"{k}={v}&");
            }

            var plainBytes = buffer.ToString(0, buffer.Length - 1).FromCharsetString(
                body.ContainsKey(Constants.Charset) ? body[Constants.Charset].ToString() : _charset);
            return Signer.Verify(
                plainBytes,
                body[Constants.Sign].ToString().FromBase64String(),
                GetHashAlgorithmName(body));
        }

        /// <summary>
        /// Get hash algorithm name
        /// </summary>
        /// <param name="body">A dictionary</param>
        /// <returns>HashAlgorithmName</returns>
        private HashAlgorithmName GetHashAlgorithmName(
            IReadOnlyDictionary<string, object> body)
        {
            var type = _signType;
            if (body.ContainsKey(Constants.SignType))
            {
                type = Enum.Parse<SignType>(body[Constants.SignType].ToString());
            }

            return type switch
            {
                SignType.RS256 => HashAlgorithmName.SHA256,
                SignType.RS384 => HashAlgorithmName.SHA384,
                SignType.RS512 => HashAlgorithmName.SHA512,
                _ => HashAlgorithmName.SHA256,
            };
        }
    }
}
