using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TrustyPay.Core.Cryptography.Http.Service
{
    public class ServiceSigner
    {
        private readonly ISignatureProvider _signer;

        public ServiceSigner(
            ISignatureProvider signer
        )
        {
            _signer = signer ?? throw new ArgumentNullException(nameof(signer));
        }

        /// <summary>
        /// Verify the signature from the request body
        /// </summary>
        /// <param name="publicKey">public key</param>
        /// <param name="path">api url</param>
        /// <param name="body">request body</param>
        /// <exception cref="ArgumentNullException">body is null</exception>
        /// <exception cref="MissingSignatureException">missing the signature in the request body</exception>
        /// <exception cref="InvalidSignatureException">the signature in the request body is not valid!!!</exception>
        public static void VerifyRequestBody(RSACryptoProvider.PublicKey publicKey, 
            string url, Dictionary<string, JToken> body)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (!body.ContainsKey("sign")
            || string.IsNullOrWhiteSpace(body["sign"].Value<string>()))
            {
                throw new MissingSignatureException();
            }
            var signature = body["sign"].Value<string>();

            var charset = "utf-8";
            if (body.ContainsKey("charset") 
            && !string.IsNullOrWhiteSpace(body["charset"].Value<string>()))
            {
                charset = body["charset"].Value<string>();
            }

            var plainBytes = GetStringForSign(url, body)
                .FromCharsetString(charset);

            var signer = new RSACryptoProvider(null, publicKey, RSASignaturePadding.Pkcs1);
            if (!signer.VerifyBase64Signature(
                plainBytes, signature, GetHashAlgorithmName(body)))
            {
                throw new InvalidSignatureException();
            }
        }

        
        public static Dictionary<string, JToken> SignResponseBody(RSACryptoProvider.PrivateKey privateKey,
            string url, object bizContent)
        {
            if (bizContent == null)
            {
                throw new ArgumentNullException(nameof(bizContent));
            }

            var body = new Dictionary<string, JToken>
            {
                {"charset", "utf-8"},
                {"bizContent", JsonConvert.SerializeObject(bizContent, Formatting.None)},
                {"timestamp", DateTimeOffset.Now.ToUnixTimeMilliseconds()},
                {"signType", SignType.RS256.ToString()}
            };

            var plainBytes = GetStringForSign(url, body)
                .FromUTF8String();
            
            var signer = new RSACryptoProvider(privateKey, null, RSASignaturePadding.Pkcs1);
            body.Add("sign", signer.SignToBase64String(plainBytes, SignType.RS256.ToHashAlgorithmName()));
            return body;
        }

        /// <summary>
        /// Get string to sign by url and body
        /// </summary>
        /// <param name="url">url path, such as /api/v1/payments</param>
        /// <param name="body">all the parameters</param>
        /// <returns>string to sign</returns>
        private static string GetStringForSign(string url, Dictionary<string, JToken> body)
        {
            var buffer = new StringBuilder(url);
            buffer.Append('?');
            foreach (var k in body.Keys.OrderBy(m => m))
            {
                var v = body[k];
                if (string.IsNullOrEmpty(k)
                    || v == null
                    || k == "sign")
                {
                    continue;
                }
                buffer.Append($"{k}={v}&");
            }
            return buffer.ToString(0, buffer.Length - 1);
        }

        /// <summary>
        /// Get hash algorithm name
        /// </summary>
        /// <param name="body">A dictionary</param>
        /// <returns>HashAlgorithmName</returns>
        private static HashAlgorithmName GetHashAlgorithmName(
            IReadOnlyDictionary<string, JToken> body)
        {
            var signType = SignType.RS256;
            if (body.ContainsKey("signType"))
            {
                signType = Enum.Parse<SignType>(body["signType"].Value<string>());
            }
            return signType.ToHashAlgorithmName();
        }
    }
}