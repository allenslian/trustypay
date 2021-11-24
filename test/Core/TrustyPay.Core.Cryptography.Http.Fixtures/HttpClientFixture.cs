using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace TrustyPay.Core.Cryptography.Http.Fixtures
{
    public class HttpClientFixture
    {
        public void ShouldBe()
        {

        }
    }

    public interface IHttpClient
    {
        Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest data);

        Task<U> GetAsync<T, U>(string url, T data);

        Task DeleteAsync<T>(string url, T data);
    }

    internal class HttpClient
    {
        protected readonly string _apiBaseUrl;

        public HttpClient(string apiBaseUrl)
        {
            _apiBaseUrl = apiBaseUrl;

        }

        public void SetSignatureProvider()
        {

        }

        public Dictionary<string, object> GenerateRequestBody<T>(T bizContent)
        {
            var map = GetBody();

            var encryption = Encrypt(bizContent);
            map.TryAdd(encryption.Key, encryption.Value);

            var signature = Sign();
            map.TryAdd(signature.Key, signature.Value);

            return map;
        }

        protected Dictionary<string, object> GetBody()
        {
            return new Dictionary<string, object>();
        }

        protected KeyValuePair<string, object> Encrypt<T>(T bizContent)
        {
            return new KeyValuePair<string, object>();
        }

        protected KeyValuePair<string, object> Sign()
        {
            return new KeyValuePair<string, object>();
        }

    }
}