
using Newtonsoft.Json;

namespace TrustyPay.Core.Cryptography.Http.Client
{
    internal class ResponseError
    {
        [JsonProperty("message", Required = Required.Always)]
        public string Message { get; set; }
    }
}