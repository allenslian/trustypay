using System;
using System.Collections.Generic;

namespace TrustyPay.Core.Cryptography.Http
{
    /// <summary>
    /// A http client base class
    /// </summary>
    public abstract class HttpClientBase : IHttpClient
    {
        /// <summary>
        /// api base url
        /// </summary>
        public string ApiBaseUrl { get; set; }

        /// <summary>
        /// A signature provider
        /// </summary>
        public ISignatureProvider Signer { protected get; set; } = null;

        #region Request

        public IReadOnlyDictionary<string, object> GenerateRequestBody<T>(string url, T bizContent)
        {
            var body = InitializeRequestBody(bizContent);
            if (body == null)
            {
                body = new Dictionary<string, object>();
            }

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
        protected abstract Dictionary<string, object> InitializeRequestBody<T>(T bizContent);

        /// <summary>
        /// If Signer is null, the method will not be invoked;
        /// Otherwise it will make one signature to request body.
        /// </summary>
        /// <param name="url">api url</param>
        /// <param name="body">request body</param>
        protected abstract void Sign(string url, ref Dictionary<string, object> body);

        #endregion

        #region Response

        public U ParseResponseBody<U>(string content)
        {
            var body = LoadResponseBody(content);
            if (body == null)
            {
                body = new Dictionary<string, object>();
            }

            if (Signer != null)
            {
                if (body.Count == 0)
                {
                    throw new MissingSignatureException();
                }

                if (!Verify(body))
                {
                    throw new InvalidSignatureException();
                }
            }

            return GetResponseBizContent<U>(body);
        }

        /// <summary>
        /// Load response body from response content.
        /// </summary>
        /// <param name="content">response content</param>
        /// <returns>A dictionary</returns>
        protected abstract Dictionary<string, object> LoadResponseBody(string content);

        /// <summary>
        /// Verify response body signature
        /// </summary>
        /// <param name="body">response body map</param>
        /// <returns>true is passed</returns>
        protected abstract bool Verify(IReadOnlyDictionary<string, object> body);
        
        /// <summary>
        /// Get response biz content
        /// </summary>
        /// <typeparam name="U">response body map</typeparam>
        /// <param name="body">response body map</param>
        /// <returns>A response biz content class</returns>
        protected abstract U GetResponseBizContent<U>(IReadOnlyDictionary<string, object> body);

        public Exception ParseResponseError(string content)
        {
            var body = LoadResponseBody(content);
            if (body == null)
            {
                body = new Dictionary<string, object>();
            }

            if (Signer != null)
            {
                if (body.Count == 0)
                {
                    throw new MissingSignatureException();
                }

                if (!Verify(body))
                {
                    throw new InvalidSignatureException();
                }
            }

            return GetResponseError(body);
        }

        /// <summary>
        /// Get response error
        /// </summary>
        /// <param name="body">response body map</param>
        /// <returns>An exception</returns>
        protected abstract Exception GetResponseError(IReadOnlyDictionary<string, object> body);

        #endregion

    }
}