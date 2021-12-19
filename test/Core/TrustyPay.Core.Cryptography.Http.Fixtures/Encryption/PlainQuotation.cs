
namespace TrustyPay.Core.Cryptography.Http.Fixtures.Encryption
{
    internal class PlainQuotation : EncryptableObjectBase
    {
        public PlainQuotation() { }

        public string Code { get; set; }

        [Encryptable]
        public string TotalAmount { get; set; }

        [Encryptable]
        public string Payer { get; set; }

        [Encryptable]
        public string PayerBankAccount { get; set; }

        public string SecretKey { get; set; }

        [Encryptable]
        public string[] Items { get; set; }

        protected override IEncryptionProvider Encryptor { get; set; }
    }
}