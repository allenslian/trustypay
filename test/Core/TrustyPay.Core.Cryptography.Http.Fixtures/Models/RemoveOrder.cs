
using Newtonsoft.Json;

namespace TrustyPay.Core.Cryptography.Http.Fixtures
{
    internal class RemoveOrder
    {
        public string Code { get; set; }
    }

    internal class RemoveOrderResult : ResponseBizContent
    {
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
