
using System;
using System.Collections.Generic;

namespace TrustyPay.Core.Cryptography.Http
{
    public interface IHttpClient
    {
        /// <summary>
        /// Api base url of the request
        /// </summary>
        string ApiBaseUrl { get; }

        /// <summary>
        /// Generate a request body
        /// </summary>
        /// <typeparam name="T">A biz content class</typeparam>
        /// <param name="url">api url</param>
        /// <param name="bizContent">biz content</param>
        /// <param name="extra">extra parameters</param>
        /// <returns>A dictionary</returns>
        IReadOnlyDictionary<string, object> GenerateRequestBody<T>(
            string url, T bizContent, IReadOnlyDictionary<string, object> extra = null);

        /// <summary>
        /// Parse http response body
        /// </summary>
        /// <param name="apiUrl">request api url</param>
        /// <param name="content">response content</param>
        /// <returns>A response biz content class</returns>
        U ParseResponseBody<U>(string apiUrl, string content);

        /// <summary>
        /// Parse http response error
        /// </summary>
        /// <param name="apiUrl">request api url</param>
        /// <param name="content">response content</param>
        /// <returns>An exception</returns>
        Exception ParseResponseError(string apiUrl, string content);
    }
}