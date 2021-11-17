using System;
using System.Text;
using Xunit;

namespace TrustyPay.Core.Cryptography.Fixtures
{
    public class SM2Fixture
    {
        [Fact]
        public void ShouldEncryptAndDecryptAsciiTextWithC1C2C3()
        {
            var plainText = "hello";
            var pubKey = "04391e5d16d0539090877b0dac4c5df5ef02ad84789b9d7e6ea79e7f8c4f5b437a088181ee625047e82e495f5e6859f7b1d797af43eb364e95199ce249a826835a".FromHexString();
            var priKey = "65886500ad98da9babd280c21362615cbe57ae4f1491cb2f9609f2ad958a906e".FromHexString();
            var sm2 = new SM2CryptoProvider(
                priKey, pubKey);
            var cipherBytes = sm2.Encrypt(Encoding.ASCII.GetBytes(plainText));
            var plainBytes = sm2.Decrypt(cipherBytes);
            Assert.Equal(plainText, Encoding.ASCII.GetString(plainBytes));
        }

        [Fact]
        public void ShouldEncryptAndDecryptAsciiTextWithC1C3C2()
        {
            var plainText = "hello";
            var pubKey = "04391e5d16d0539090877b0dac4c5df5ef02ad84789b9d7e6ea79e7f8c4f5b437a088181ee625047e82e495f5e6859f7b1d797af43eb364e95199ce249a826835a".FromHexString();
            var priKey = "65886500ad98da9babd280c21362615cbe57ae4f1491cb2f9609f2ad958a906e".FromHexString();
            var sm2 = new SM2CryptoProvider(
                priKey, pubKey, SM2CryptoProvider.CipherFormat.C1C3C2);
            var cipherBytes = sm2.Encrypt(Encoding.ASCII.GetBytes(plainText));
            var plainBytes = sm2.Decrypt(cipherBytes);
            Assert.Equal(plainText, Encoding.ASCII.GetString(plainBytes));
        }

        [Fact]
        public void ShouldFailToInitialize()
        {
            var pubKey = "04391e5d16d0539090877b0dac4c5df5ef02ad84789b9d7e6ea79e7f8c4f5b437a088181ee625047e82e495f5e6859f7b1d797af43eb364e95199ce249a826835a".FromHexString();
            var priKey = "65886500ad98da9babd280c21362615cbe57ae4f1491cb2f9609f2ad958a906e".FromHexString();
            Assert.Throws<ArgumentNullException>(() =>
            {
                new SM2CryptoProvider(null, pubKey);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                new SM2CryptoProvider(Array.Empty<byte>(), pubKey);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                new SM2CryptoProvider(priKey, null);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                new SM2CryptoProvider(priKey, Array.Empty<byte>());
            });
        }

        [Fact]
        public void ShouldFailToEncrypt()
        {
            var pubKey = "04391e5d16d0539090877b0dac4c5df5ef02ad84789b9d7e6ea79e7f8c4f5b437a088181ee625047e82e495f5e6859f7b1d797af43eb364e95199ce249a826835a".FromHexString();
            var priKey = "65886500ad98da9babd280c21362615cbe57ae4f1491cb2f9609f2ad958a906e".FromHexString();
            IEncryptionProvider sm2 = new SM2CryptoProvider(priKey, pubKey);
            Assert.Throws<ArgumentNullException>(() =>
            {
                sm2.Encrypt(null);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                sm2.Encrypt(Array.Empty<byte>());
            });
        }

        [Fact]
        public void ShouldFailToDecrypt()
        {
            var pubKey = "04391e5d16d0539090877b0dac4c5df5ef02ad84789b9d7e6ea79e7f8c4f5b437a088181ee625047e82e495f5e6859f7b1d797af43eb364e95199ce249a826835a".FromHexString();
            var priKey = "65886500ad98da9babd280c21362615cbe57ae4f1491cb2f9609f2ad958a906e".FromHexString();
            IEncryptionProvider sm2 = new SM2CryptoProvider(priKey, pubKey);
            Assert.Throws<ArgumentNullException>(() =>
            {
                sm2.Decrypt(null);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                sm2.Decrypt(Array.Empty<byte>());
            });
        }
    }
}