using System.Collections.Generic;


namespace TrustyPay.Core.Cryptography.Http
{
    public abstract class HttpRequestBase<T> : IHttpRequest<T> where T : class
    {
        protected HttpRequestBase(
            string apiBaseUrl)
        {
            ApiBaseUrl = apiBaseUrl ?? string.Empty;
        }

        public string ApiBaseUrl { get; private set; }

        /// <summary>
        /// An ecryption provider
        /// </summary>
        public IEncryptionProvider Encryptor { protected get; set; } = null;

        /// <summary>
        /// A signature provider
        /// </summary>
        public ISignatureProvider Signer { protected get; set; } = null;

        public IReadOnlyDictionary<string, object> GenerateBody(string url, T bizContent)
        {
            var body = InitializeBody();
            if (body == null)
            {
                body = new Dictionary<string, object>();
            }

            Encrypt(bizContent, ref body);

            if (Signer != null)
            {
                Sign(url, ref body);
            }
            return body;
        }

        /// <summary>
        /// Initialize request body
        /// </summary>
        /// <returns>request body</returns>
        protected abstract Dictionary<string, object> InitializeBody();

        /// <summary>
        /// If Encryptor is null, it will return bizContent directly;
        /// Otherwise it will encrypt some fields or all fields in the bizContent.
        /// </summary>
        /// <param name="bizContent">biz content</param>
        /// <param name="body">request body</param>
        protected abstract void Encrypt(T bizContent, ref Dictionary<string, object> body);

        /// <summary>
        /// If Signer is null, the method will not be invoked;
        /// Otherwise it will make one signature to request body.
        /// </summary>
        /// <param name="url">api url</param>
        /// <param name="body">request body</param>
        protected abstract void Sign(string url, ref Dictionary<string, object> body);
    }
}