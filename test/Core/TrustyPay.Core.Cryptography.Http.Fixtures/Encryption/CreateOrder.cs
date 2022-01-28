
using System;

namespace TrustyPay.Core.Cryptography.Http.Fixtures.Encryption
{
    internal class CreateOrder
    {
        public string Code { get; set; }

        [Encryptable]
        public string TotalAmount { get; set; }

        [Encryptable]
        public string Payer { get; set; }

        [Encryptable]
        public string PayerBankAccount { get; set; }

        public string SecretKey { get; set; }

        [Encryptable]
        public OrderLineItem[] Items { get; set; }

        internal class OrderLineItem
        {
            private string _key;

            public int LineNo { get; set; }

            [Encryptable]
            public string Amount { get; set; }

            [Encryptable]
            public string Payee { get; set; }

            [Encryptable]
            public string PayeeAccountName { get; set; }

            [Encryptable]
            public string PayeeAccountNO { get; set; }
        }
    }
}