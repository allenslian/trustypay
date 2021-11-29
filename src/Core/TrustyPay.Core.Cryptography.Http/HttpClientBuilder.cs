


using System.Security.Cryptography;

namespace TrustyPay.Core.Cryptography.Http
{
    public class HttpClientBuilder<T> where T : HttpClientBase, new()
    {
        protected HttpClientBase _client;

        public HttpClientBuilder()
        {
            _client = new T();
        }

        public HttpClientBuilder<T> AddApiBaseUrl(string apiBaseUrl)
        {
            _client.ApiBaseUrl = apiBaseUrl;
            return this;
        }

        public HttpClientBuilder<T> WithSigner(ISignatureProvider provider)
        {
            _client.Signer = provider;
            return this;
        }

        public HttpClientBuilder<T> WithRSASigner(
            RSACryptoProvider.PrivateKey privateKey,
            RSACryptoProvider.PublicKey publicKey,
            RSASignaturePadding padding = null,
            RSACryptoProvider.KeySizes keySize = RSACryptoProvider.KeySizes.RSA2048)
        {
            _client.Signer = new RSACryptoProvider(
                privateKey, publicKey, padding, keySize);
            return this;
        }

        public IHttpClient Build()
        {
            return _client;
        }
    }
}
