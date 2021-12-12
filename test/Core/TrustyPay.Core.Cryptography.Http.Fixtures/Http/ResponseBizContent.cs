
using Newtonsoft.Json;

namespace TrustyPay.Core.Cryptography.Http.Fixtures.Http
{
    internal class ResponseBizContent
    {
        [JsonProperty("return_code")]
        public string ReturnCode { get; set; }

        [JsonProperty("return_msg")]
        public string ReturnMsg { get; set; }
    }
}