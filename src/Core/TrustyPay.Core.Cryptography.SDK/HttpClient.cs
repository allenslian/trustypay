using System;
using TrustyPay.Core.Cryptography;

namespace TrustyPay.Core.Cryptography.SDK
{
    /// <summary>
    /// A http client for communicating with TrustyPay web api.
    /// </summary>
    public sealed class HttpClient
    {
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
        /// Your rsa private key
        /// </summary>
        private readonly RSACryptoProvider.PrivateKey _privateKey;

        /// <summary>
        /// TrustyPay rsa public key
        /// </summary>
        private readonly RSACryptoProvider.PublicKey _publicKey;

        public HttpClient()
        {
            
        }

        
    }
}
