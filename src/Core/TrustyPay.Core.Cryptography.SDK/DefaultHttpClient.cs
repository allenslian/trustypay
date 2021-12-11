using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using TrustyPay.Core.Cryptography;
using TrustyPay.Core.Cryptography.Http;

namespace TrustyPay.Core.Cryptography.SDK
{
    /// <summary>
    /// A http client for communicating with TrustyPay web api.
    /// </summary>
    public sealed class DefaultHttpClient : HttpClientBase
    {
        public enum SignType
        {
            RS256 = 256,

            RS384 = 384,

            RS512 = 512
        }

        /// <summary>
        /// Api base url
        /// </summary>
        private readonly string _apiBaseUrl;

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
            SignType signType = SignType.RS256, string charset = "utf-8", bool hasTimestamp = true)
        {
            _appId = appId ?? throw new ArgumentNullException(nameof(appId));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _apiBaseUrl = apiBaseUrl;
            _signType = signType;
            _charset = charset;
            _hasTimestamp = hasTimestamp;
        }

        protected override U GetResponseBizContent<U>(IReadOnlyDictionary<string, object> body)
        {
            if (!body.ContainsKey("result"))
            {
                throw new ArgumentException("No any result!!!", nameof(body));
            }
            return JsonConvert.DeserializeObject<U>(body["result"].ToString());
        }

        protected override Exception GetResponseError(IReadOnlyDictionary<string, object> body)
        {
            var error = JsonConvert.DeserializeObject<ResponseBizContent>(
                body["result"].ToString());
            return new Exception(error.ReturnMsg);
        }

        protected override Dictionary<string, object> InitializeRequestBody<T>(T bizContent, IReadOnlyDictionary<string, object> extra = null)
        {
            var body = new Dictionary<string, object>
            {
                {"appId", _appId},
                {"apiKey", _apiKey},
                {"charset", _charset},
                {"bizContent", IsPrimitiveType<T>() ? bizContent : JsonConvert.SerializeObject(bizContent)},
                {"signType", _signType}
            };

            if (_hasTimestamp)
            {
                body.Add("timestamp", DateTimeOffset.Now.ToUnixTimeMilliseconds());
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
            foreach (var kv in body)
            {
                if (string.IsNullOrEmpty(kv.Key)
                    || kv.Value == null
                    || kv.Key == "sign")
                {
                    continue;
                }
                buffer.Append($"{kv.Key}={kv.Value}&");
            }

            var plainBytes = buffer.ToString(0, buffer.Length - 1).FromCharsetString(
                body.ContainsKey("charset") ? body["charset"].ToString() : _charset);
            body["sign"] = Signer.Sign(plainBytes, GetHashAlgorithmName((SignType)body["signType"])).ToBase64String();
        }

        protected override bool Verify(IReadOnlyDictionary<string, object> body)
        {
            if (body.ContainsKey("sign"))
            {
                throw new MissingSignatureException();
            }

            var buffer = new StringBuilder();
            foreach (var kv in body)
            {
                if (string.IsNullOrEmpty(kv.Key)
                    || kv.Value == null
                    || kv.Key == "sign")
                {
                    continue;
                }
                buffer.Append($"{kv.Key}={kv.Value}&");
            }

            var plainBytes = buffer.ToString(0, buffer.Length - 1).FromCharsetString(
                body.ContainsKey("charset") ? body["charset"].ToString() : _charset);
            return Signer.Verify(
                plainBytes, 
                body["sign"].ToString().FromBase64String(),
                GetHashAlgorithmName((SignType)body["signType"]));
        }

        /// <summary>
        /// Get hash algorithm name
        /// </summary>
        /// <param name="type">sign type</param>
        /// <returns>HashAlgorithmName</returns>
        private static HashAlgorithmName GetHashAlgorithmName(SignType type)
        {
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
