using Newtonsoft.Json;

namespace TrustyPay.Core.Cryptography.Http.Fixtures.Http
{
    internal class TerminateOrder
    {
        public string Code { get; set; }
    }

    internal class TernimateOrderResult : ResponseBizContent
    {
        [JsonProperty("trxTimestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("trxTime")]
        public string Time { get; set; }

        [JsonProperty("msg_id")]
        public string MsgId { get; set; }

        [JsonProperty("trxDate")]
        public string Date { get; set; }

        [JsonProperty("corp_serno")]
        public string SeqNo { get; set; }
    }
}