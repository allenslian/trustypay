using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TrustyPay.Core.Cryptography.Http.Service
{
    internal class ServiceSigner
    {
        /// <summary>
        /// Verify the signature from the request body
        /// </summary>
        /// <param name="publicKey">public key</param>
        /// <param name="path">api url</param>
        /// <param name="body">request body</param>
        /// <exception cref="ArgumentNullException">body is null</exception>
        /// <exception cref="MissingSignatureException">missing the signature in the request body</exception>
        /// <exception cref="InvalidSignatureException">the signature in the request body is not valid!!!</exception>
        public static void VerifyRequestBody(
            RSACryptoProvider.PublicKey publicKey,
            string url, Dictionary<string, JToken> body)
        {
            // if (body == null)
            // {
            //     throw new ArgumentNullException(nameof(body));
            // }

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


        public static Dictionary<string, JToken> SignResponseBody(
            RSACryptoProvider.PrivateKey privateKey,
            string url, JToken bizContent)
        {
            // if (bizContent == null)
            // {
            //     throw new ArgumentNullException(nameof(bizContent));
            // }

            var body = new Dictionary<string, JToken>
            {
                {"charset", "utf-8"},
                {"result", IsPrimitiveType(bizContent) ? bizContent : JsonConvert.SerializeObject(bizContent, Formatting.None)},
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
                    || v.ToString() == string.Empty
                    || v.ToString().Equals("null", StringComparison.CurrentCultureIgnoreCase)
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

        /// <summary>
        /// Whether biz content type is primitive type?
        /// </summary>
        /// <typeparam name="T">biz Content class</typeparam>
        /// <returns>true/false</returns>
        private static bool IsPrimitiveType(JToken token)
        {
            if (token == null)
            {
                return true;
            }
            return token.Type != JTokenType.Object && token.Type != JTokenType.Array;
        }
    }
}