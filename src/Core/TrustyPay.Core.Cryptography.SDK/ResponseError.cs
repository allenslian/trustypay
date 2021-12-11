
using Newtonsoft.Json;

namespace TrustyPay.Core.Cryptography.SDK
{
    internal class ResponseError
    {
        [JsonProperty("message")]
        public string Message { get; }
    }
}