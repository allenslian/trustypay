
using Newtonsoft.Json;

namespace TrustyPay.Core.Cryptography.Http.Fixtures
{
    internal class CreateOrderResult : ResponseBizContent
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("orderCurr")]
        public string Currency { get; set; }

        [JsonProperty("payAmount")]
        public string PayAmount { get; set; }

        [JsonProperty("partnerSeq")]
        public string SeqNo { get; set; }
    }
}