using System.Collections.Generic;

namespace TrustyPay.Core.Cryptography.Http
{
    public interface IHttpRequest<T> where T : class
    {
        /// <summary>
        /// Api base url of the request
        /// </summary>
        string ApiBaseUrl { get; }

        /// <summary>
        /// Generate a request body
        /// </summary>
        /// <typeparam name="T">A class</typeparam>
        /// <param name="url">api url</param>
        /// <param name="bizContent">biz content</param>
        /// <returns>A dictionary</returns>
        IReadOnlyDictionary<string, object> GenerateBody(string url, T bizContent);
    }
}
