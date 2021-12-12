using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;

namespace TrustyPay.Core.Cryptography.Http
{
    public static class HttpClientExtension
    {
        /// <summary>
        /// Send post request
        /// </summary>
        /// <typeparam name="T">request biz content class</typeparam>
        /// <typeparam name="U">response biz content class</typeparam>
        /// <param name="client">A http client instance</param>
        /// <param name="apiUrl">relative api url</param>
        /// <param name="bizContent">request biz content</param>
        /// <param name="extra">extra parameters</param>
        /// <returns>A response biz content</returns>
        /// <exception cref="ArgumentNullException">A http client instance is null</exception>
        public static async Task<U> PostJsonAsync<T, U>(
            this IHttpClient client, string apiUrl, T bizContent, IReadOnlyDictionary<string, object> extra = null)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var url = ConcatenateApiUrl(client.ApiBaseUrl, apiUrl, null);

            try
            {
                var response = await url.PostJsonAsync(
                    client.GenerateRequestBody(apiUrl, bizContent, extra)
                );
                return client.ParseResponseBody<U>(apiUrl, await response.GetStringAsync());
            }
            catch (FlurlHttpException ex)
            {
                throw client.ParseResponseError(apiUrl, await ex.GetResponseStringAsync());
            }
        }

        /// <summary>
        /// Send get request
        /// </summary>
        /// <typeparam name="T">request biz content class</typeparam>
        /// <typeparam name="U">response biz content class</typeparam>
        /// <param name="client">A http client instance</param>
        /// <param name="apiUrl">relative api url</param>
        /// <param name="bizContent">request biz content</param>
        /// <param name="extra">extra parameters</param>
        /// <returns>A response biz content</returns>
        /// <exception cref="ArgumentNullException">A http client instance is null</exception>
        public static async Task<U> GetAsync<T, U>(
            this IHttpClient client, string apiUrl, T bizContent, IReadOnlyDictionary<string, object> extra = null)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var url = ConcatenateApiUrl(
                client.ApiBaseUrl,
                apiUrl,
                client.GenerateRequestBody(apiUrl, bizContent, extra));
            try
            {
                var response = await url.GetAsync();
                return client.ParseResponseBody<U>(apiUrl, await response.GetStringAsync());
            }
            catch (FlurlHttpException ex)
            {
                throw client.ParseResponseError(apiUrl, await ex.GetResponseStringAsync());
            }
        }

        /// <summary>
        /// Send put request
        /// </summary>
        /// <typeparam name="T">request biz content class</typeparam>
        /// <typeparam name="U">response biz content class</typeparam>
        /// <param name="client">A http client instance</param>
        /// <param name="apiUrl">relative api url</param>
        /// <param name="bizContent">request biz content</param>
        /// <param name="extra">extra parameters</param>
        /// <returns>A response biz content</returns>
        /// <exception cref="ArgumentNullException">A http client instance is null</exception>
        public static async Task<U> PutJsonAsync<T, U>(
            this IHttpClient client, string apiUrl, T bizContent, IReadOnlyDictionary<string, object> extra = null)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var url = ConcatenateApiUrl(client.ApiBaseUrl, apiUrl, null);

            try
            {
                var response = await url.PutJsonAsync(
                    client.GenerateRequestBody(apiUrl, bizContent, extra)
                );
                return client.ParseResponseBody<U>(apiUrl, await response.GetStringAsync());
            }
            catch (FlurlHttpException ex)
            {
                throw client.ParseResponseError(apiUrl, await ex.GetResponseStringAsync());
            }
        }

        /// <summary>
        /// Send delete request
        /// </summary>
        /// <typeparam name="T">request biz content class</typeparam>
        /// <typeparam name="U">response biz content class</typeparam>
        /// <param name="client">A http client instance</param>
        /// <param name="apiUrl">relative api url</param>
        /// <param name="bizContent">request biz content</param>
        /// <param name="extra">extra parameters</param>
        /// <returns>A response biz content</returns>
        /// <exception cref="ArgumentNullException">A http client instance is null</exception>
        public static async Task<U> DeleteAsync<T, U>(
            this IHttpClient client, string apiUrl, T bizContent, IReadOnlyDictionary<string, object> extra = null)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var url = ConcatenateApiUrl(
                client.ApiBaseUrl,
                apiUrl,
                client.GenerateRequestBody(apiUrl, bizContent, extra));
            try
            {
                var response = await url.DeleteAsync();
                return client.ParseResponseBody<U>(apiUrl, await response.GetStringAsync());
            }
            catch (FlurlHttpException ex)
            {
                throw client.ParseResponseError(apiUrl, await ex.GetResponseStringAsync());
            }
        }

        /// <summary>
        /// Concatenate api url to absolute api url.
        /// </summary>
        /// <param name="apiBaseUrl">api base url</param>
        /// <param name="apiUrl">relative api url</param>
        /// <param name="body">request body map</param>
        /// <returns>A absolute api url</returns>
        /// <exception cref="ArgumentException">absolute api url is null</exception>
        private static string ConcatenateApiUrl(
            string apiBaseUrl,
            string apiUrl,
            IReadOnlyDictionary<string, object> body)
        {
            var url = new Flurl.Url(apiBaseUrl)
                .AppendPathSegment(apiUrl);

            var absoluteUrl = url.ToString();
            if (string.IsNullOrEmpty(absoluteUrl)
                || !absoluteUrl.StartsWith("http"))
            {
                throw new ArgumentException("An invalid url!!!", nameof(apiUrl));
            }

            if (body == null)
            {
                return absoluteUrl;
            }

            absoluteUrl = body.Aggregate(url, (acc, kv) =>
            {
                acc.SetQueryParam(kv.Key, kv.Value);
                return acc;
            }).ToString();
            return absoluteUrl;
        }
    }
}