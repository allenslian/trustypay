using System;
using System.Security.Cryptography;

namespace TrustyPay.Core.Cryptography.Http
{
    public class HttpClientBuilder
    {
        protected HttpClientBase _client;

        public HttpClientBuilder(HttpClientBase client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public HttpClientBuilder WithSigner(ISignatureProvider provider)
        {
            _client.Signer = provider;
            return this;
        }

        public HttpClientBuilder WithRSASigner(
            RSACryptoProvider.PrivateKey privateKey,
            RSACryptoProvider.PublicKey publicKey,
            RSASignaturePadding padding = null,
            RSACryptoProvider.KeySizes keySize = RSACryptoProvider.KeySizes.RSA2048)
        {
            _client.Signer = new RSACryptoProvider(
                privateKey,
                publicKey,
                padding,
                keySize);
            return this;
        }

        public HttpClientBuilder WithRSASigner(
            Tuple<string, RSACryptoProvider.PrivateKeyFormat> privateKey,
            Tuple<string, RSACryptoProvider.PublicKeyFormat> publicKey,
            RSASignaturePadding padding = null,
            RSACryptoProvider.KeySizes keySize = RSACryptoProvider.KeySizes.RSA2048)
        {
            _client.Signer = new RSACryptoProvider(
                RSAKeyFactory.ImportPrivateKeyFromBase64String(privateKey.Item1, privateKey.Item2),
                RSAKeyFactory.ImportPublicKeyFromBase64String(publicKey.Item1, publicKey.Item2),
                padding,
                keySize);
            return this;
        }

        public IHttpClient Build()
        {
            return _client;
        }
    }
}
