

namespace TrustyPay.Core.Cryptography.Http.Fixtures.Encryption
{
    internal class CreateOrder : EncryptableObjectBase
    {
        public CreateOrder()
        {
            SecretKey = "hellowor";
            Encryptor = new TripleDESEncryptionProvider(
                SecretKey.FromASCIIString(),
                "0000000000000000".FromHexString()
            );
        }

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

        protected override IEncryptionProvider Encryptor { get; set; }

        internal class OrderLineItem : EncryptableObjectBase
        {
            private string _key;

            public OrderLineItem(string secretKey)
            {
                _key = secretKey;
                Encryptor = new TripleDESEncryptionProvider(
                    _key.FromASCIIString(),
                    "0000000000000000".FromHexString()
                );
            }

            public int LineNo { get; set; }

            [Encryptable]
            public string Amount { get; set; }

            [Encryptable]
            public string Payee { get; set; }

            [Encryptable]
            public string PayeeAccountName { get; set; }

            [Encryptable]
            public string PayeeAccountNO { get; set; }

            protected override IEncryptionProvider Encryptor { get; set; }
        }
    }
}