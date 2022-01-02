
namespace TrustyPay.Core.Cryptography.Http.Fixtures.Client
{
    internal class CreateOrder
    {
        public string Code { get; set; }

        public decimal TotalAmount { get; set; }

        public string Payer { get; set; }

        public string PayerBankAccount { get; set; }

        public OrderLineItem[] Items { get; set; }

        internal class OrderLineItem
        {
            public int LineNo { get; set; }

            public decimal Amount { get; set; }

            public string Payee { get; set; }

            public string PayeeAccountName { get; set; }

            public string PayeeAccountNO { get; set; }
        }
    }
}