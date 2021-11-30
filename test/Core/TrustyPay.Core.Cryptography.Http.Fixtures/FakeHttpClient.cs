using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace TrustyPay.Core.Cryptography.Http.Fixtures
{
    public class FakeHttpClient : HttpClientBase
    {
        private string _apiKey;

        public FakeHttpClient(string apiBaseUrl, string apiKey)
        {
            ApiBaseUrl = apiBaseUrl ?? string.Empty;
            _apiKey = apiKey;
        }

        protected override U GetResponseBizContent<U>(IReadOnlyDictionary<string, object> body)
        {
            return JsonConvert.DeserializeObject<U>(body["response_biz_content"].ToString());
        }

        protected override Exception GetResponseError(IReadOnlyDictionary<string, object> body)
        {
            return new Exception(body["response_biz_content"].ToString());
        }

        protected override Dictionary<string, object> InitializeRequestBody<T>(
            T bizContent, IReadOnlyDictionary<string, object> extra = null)
        {
            var body = new Dictionary<string, object>
            {
                {"api_key", _apiKey},
                {"charset", "utf-8"},
                {"biz_content", IsPrimitiveType<T>() ? bizContent : JsonConvert.SerializeObject(bizContent)},
            };
            if (extra != null)
            {
                foreach (var kv in extra)
                {
                    body.TryAdd(kv.Key, kv.Value);
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
                    || kv.Value == null)
                {
                    continue;
                }
                buffer.Append($"{kv.Key}={kv.Value}&");
            }

            var text = buffer.ToString(0, buffer.Length - 1);
            body.Add("sign", Signer.Sign(text.FromUTF8String(), HashAlgorithmName.SHA256).ToBase64String());
        }

        protected override bool Verify(IReadOnlyDictionary<string, object> body)
        {
            var buffer = new StringBuilder();
            foreach (var kv in body)
            {
                if (string.IsNullOrEmpty(kv.Key)
                    || kv.Value == null
                    || kv.Key != "sign")
                {
                    continue;
                }
                buffer.Append($"{kv.Key}={kv.Value}&");
            }
            var text = buffer.ToString(0, buffer.Length - 1);
            return Signer.VerifyBase64Signature(
                text.FromUTF8String(),
                body["sign"].ToString(),
                HashAlgorithmName.SHA256);
        }
    }
}