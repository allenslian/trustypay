
using Newtonsoft.Json;

namespace TrustyPay.Core.Cryptography.SDK
{
    internal class ResponseError
    {
        [JsonProperty("message", Required = Required.Always)]
        public string Message { get; set; }
    }
}