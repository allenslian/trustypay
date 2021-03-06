


using Xunit;

namespace TrustyPay.Core.Cryptography.Http.Fixtures.Encryption
{
    public class HttpClientFixture
    {
        [Fact]
        public void EncryptCreateOrder()
        {
            var order = new CreateOrder
            {
                Code = "01",
                TotalAmount = "20.00",
                Payer = "allen",
                PayerBankAccount = "12345678"
            };
            order.Items = new CreateOrder.OrderLineItem[]{
                new CreateOrder.OrderLineItem{
                    LineNo = 1,
                    Amount = "12.00",
                    Payee = "bill",
                    PayeeAccountName = "bill",
                    PayeeAccountNO = "87654321"
                },
                new CreateOrder.OrderLineItem{
                    LineNo = 2,
                    Amount = "8.00",
                    Payee = "world",
                    PayeeAccountName = "world",
                    PayeeAccountNO = "87654322"
                }
            };

            var encryptor = new ObjectEncryptor(
                new TripleDESEncryptionProvider(
                    "hellowor".FromASCIIString(),
                    "0000000000000000".FromHexString()
                )
            );
            encryptor.Encrypt(order);
            Assert.Equal("01", order.Code);
            Assert.Equal("xAkKhcFz/io=", order.TotalAmount);
            Assert.Equal(1, order.Items[0].LineNo);
            Assert.Equal("Q2O+dtmfiuQ=", order.Items[0].Payee);
            Assert.Equal(2, order.Items[1].LineNo);
            Assert.Equal("CphoRPfrKv0=", order.Items[1].Payee);

            encryptor.Decrypt(order);
            Assert.Equal("01", order.Code);
            Assert.Equal("20.00", order.TotalAmount);
            Assert.Equal(1, order.Items[0].LineNo);
            Assert.Equal("bill", order.Items[0].Payee);
            Assert.Equal(2, order.Items[1].LineNo);
            Assert.Equal("world", order.Items[1].Payee);
        }

        [Fact]
        public void EncryptQuotation()
        {
            var quotation = new Quotation
            {
                Code = "01",
                TotalAmount = "20.00",
                Payer = null,
                PayerBankAccount = ""
            };
            quotation.Items = new string[] {
                "1|bill|87654321|12.00",
                "2|world|87654322|8.00"
            };
            quotation.ItemWithoutEncryption = new Quotation.QuotationItem();

            var encryptor = new ObjectEncryptor(
                new AESEncryptionProvider(
                    "helloworld!!!!!!".FromASCIIString(),
                    "00000000000000000000000000000000".FromHexString()
                )
            );
            encryptor.Encrypt(quotation);

            Assert.Equal("01", quotation.Code);
            Assert.Equal("DddQjA+a5pSPhYM9NROUog==", quotation.TotalAmount);
            Assert.Null(quotation.Payer);
            Assert.Equal("", quotation.PayerBankAccount);
            Assert.Equal("n7I5b9TB1t+3ztIz0qudGI0RN/y9X2YT0e/Bh89ZEqM=", quotation.Items[0]);
            Assert.Equal("NLzLajeXT7FKq9YntxsCxFRB3ZdqiYagOVxvdVy7NpA=", quotation.Items[1]);

            encryptor.Decrypt(quotation);
            Assert.Equal("20.00", quotation.TotalAmount);
            Assert.Null(quotation.Payer);
            Assert.Equal("", quotation.PayerBankAccount);
            Assert.Equal("1|bill|87654321|12.00", quotation.Items[0]);
            Assert.Equal("2|world|87654322|8.00", quotation.Items[1]);
        }

        [Fact]
        public void DonotEncryptPlainQuotation()
        {
            var quotation = new PlainQuotation
            {
                Code = "01",
                TotalAmount = "20.00",
                Payer = "allen",
                PayerBankAccount = "12345678"
            };
            quotation.Items = new string[] {
                "1|bill|87654321|12.00",
                "2|world|87654322|8.00"
            };

            var encryptor = new ObjectEncryptor(
                new AESEncryptionProvider(
                    "helloworld".FromASCIIString(),
                    "0000000000000000".FromHexString()
                )
            );
            encryptor.Encrypt(quotation);

            Assert.Equal("01", quotation.Code);
            Assert.Equal("YrlwmrwfhwQDwcRUiYmWzg==", quotation.TotalAmount);
            Assert.Equal("q7d58MknSFBb1VO3+t3nTv8Z7fLaJIG1jtfFi4dMcb8=", quotation.Items[0]);
            Assert.Equal("QuYpRLr18r2bFmE2XzXKFd+qDC2iaB5u0Z2k/13pWVA=", quotation.Items[1]);

            encryptor.Decrypt(quotation);
            Assert.Equal("20.00", quotation.TotalAmount);
            Assert.Equal("1|bill|87654321|12.00", quotation.Items[0]);
            Assert.Equal("2|world|87654322|8.00", quotation.Items[1]);
        }
    }
}