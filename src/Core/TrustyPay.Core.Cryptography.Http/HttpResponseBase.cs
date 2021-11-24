using System;
using System.Collections.Generic;

namespace TrustyPay.Core.Cryptography.Http
{
    public abstract class HttpResponseBase<U> : IHttpResponse<U> where U : class
    {
        protected HttpResponseBase()
        {

        }

        /// <summary>
        /// An ecryption provider
        /// </summary>
        public IEncryptionProvider Encryptor { protected get; set; } = null;

        /// <summary>
        /// A signature provider
        /// </summary>
        public ISignatureProvider Signer { protected get; set; } = null;

        public U ParseBody(string content)
        {
            var body = LoadBodyFrom(content);
            if (body == null)
            {
                body = new Dictionary<string, object>();
            }

            if (Signer != null)
            {
                if (body.Count == 0)
                {
                    throw new ArgumentException("An INVALID signature!");
                }

                if (!Verify(ref body))
                {
                    throw new ArgumentException("An INVALID signature!");
                }
            }

            return Decrypt(ref body);
        }

        /// <summary>
        /// Load body from response content.
        /// </summary>
        /// <param name="content">response content</param>
        /// <returns>A dictionary</returns>
        protected abstract Dictionary<string, object> LoadBodyFrom(string content);

        /// <summary>
        /// If Encryptor is null, it will return U directly;
        /// Otherwise it will decrypt some fields or all fields in the body.
        /// </summary>
        /// <param name="body"></param>
        /// <returns>response content instance</returns>
        protected abstract U Decrypt(ref Dictionary<string, object> body);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        protected abstract bool Verify(ref Dictionary<string, object> body);
    }
}