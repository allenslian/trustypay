
using Newtonsoft.Json;

namespace TrustyPay.Core.Cryptography.Http.Fixtures.Http
{
    internal class OrderCriteria
    {
        [JsonProperty("order_code")]
        public string OrderCode { get; set; }
    }
}