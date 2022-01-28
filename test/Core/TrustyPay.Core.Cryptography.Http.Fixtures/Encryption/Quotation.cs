
namespace TrustyPay.Core.Cryptography.Http.Fixtures.Encryption
{
    internal class Quotation
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
        public string[] Items { get; set; }

        [Encryptable]
        public QuotationItem ItemWithoutEncryption { get; set; }

        internal class QuotationItem
        {

        }
    }
}