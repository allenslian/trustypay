using System;

namespace TrustyPay.Core.Cryptography.Http.Fixtures.SDK
{
    internal class OrderResult
    {
        public string Id { get; set; }

        public decimal Amount { get; set; }

        public string Payer { get; set; }

        public string Remarks { get; set; }

        public int Status { get; set; }

        public DateTime DateCreated { get; set; }
    }
}