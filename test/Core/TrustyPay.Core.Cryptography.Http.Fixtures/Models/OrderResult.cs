
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrustyPay.Core.Cryptography.Http.Fixtures
{
    internal class OrderResult : ResponseBizContent
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("sumPayamt")]
        public string TotalAmount { get; set; }

        [JsonProperty("payeeList")]
        public List<Payee> Payees { get; set; }

        internal class Payee
        {
            [JsonProperty("payAmount")]
            public string Amount { get; set; }

            [JsonProperty("payeeCompanyName")]
            public string Company { get; set; }

            [JsonProperty("payeeAccno")]
            public string AccountNo { get; set; }

            [JsonProperty("payeeAddress")]
            public string Address { get; set; }

            [JsonProperty("payeeOrgcode")]
            public string OrgCode { get; set; }
        }
    } 
}