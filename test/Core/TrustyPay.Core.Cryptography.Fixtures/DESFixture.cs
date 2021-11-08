using System;
using System.Text;
using Xunit;
using System.Security.Cryptography;

namespace TrustyPay.Core.Cryptography.Fixtures
{
    public class DESFixture
    {
        [Fact]
        public void TestInitializeDESEncryptionProviderWithoutSecretKey()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var provider = new DESEncryptionProvider(null, null);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                var provider = new DESEncryptionProvider(Array.Empty<byte>(), null);
            });
        }

        [Fact]
        public void ShouldEncryptAndDecryptAsciiText()
        {
            var plainText = "hello";
            var secretKey = Encoding.ASCII.GetBytes("a Secret Key!!!!!");
            var provider = new DESEncryptionProvider(secretKey, null);
            var cipherBytes = provider.Encrypt(Encoding.ASCII.GetBytes(plainText));
            var plainBytes = provider.Decrypt(cipherBytes);
            Assert.Equal(plainText, Encoding.ASCII.GetString(plainBytes));
        }

        [Fact]
        public void ShouldEncryptAndDecryptUTF8Text()
        {
            var plainText = "hello, 世界";
            var secretKey = Encoding.ASCII.GetBytes("a Secret Key");
            var provider = new DESEncryptionProvider(secretKey, null);
            var cipherBytes = provider.Encrypt(Encoding.UTF8.GetBytes(plainText));
            var plainBytes = provider.Decrypt(cipherBytes);
            Assert.Equal(plainText, Encoding.UTF8.GetString(plainBytes));
        }

        [Fact]
        public void ShouldEncryptAndDecryptGBKText()
        {
            var plainText = "hello, 世界";
            var secretKey = Encoding.ASCII.GetBytes("123456781234567812345678");
            var iv = Encoding.ASCII.GetBytes("12345678");
            
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var provider = new DESEncryptionProvider(secretKey, iv);
            var cipherBytes = provider.Encrypt(Encoding.GetEncoding("GBK").GetBytes(plainText));
            var plainBytes = provider.Decrypt(cipherBytes);
            Assert.Equal(plainText, Encoding.GetEncoding("GBK").GetString(plainBytes));
        }

        [Fact]
        public void ShouldEncryptAndDecryptWithBase64()
        {
            var plainText = "hello, 世界";
            var secretKey = Encoding.ASCII.GetBytes("a Secret Key");
            
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var provider = new DESEncryptionProvider(secretKey, null);
            var cipher = provider.EncryptToBase64String(
                Encoding.GetEncoding("GBK").GetBytes(plainText));
            var plainBytes = provider.DecryptFromBase64String(cipher);
            Assert.Equal(plainText, Encoding.GetEncoding("GBK").GetString(plainBytes));
        }

        [Theory]
        [InlineData(CipherMode.CBC)]
        [InlineData(CipherMode.ECB)]
        [InlineData(CipherMode.CFB)]
        public void ShouldEncryptAndDecryptWithCipherMode(CipherMode mode)
        {
            var plainText = "hello";
            var secretKey = Encoding.ASCII.GetBytes("a Secret Key!!!!!");
            var provider = new DESEncryptionProvider(secretKey, null, mode);
            var cipherBytes = provider.Encrypt(Encoding.ASCII.GetBytes(plainText));
            var plainBytes = provider.Decrypt(cipherBytes);
            Assert.Equal(plainText, Encoding.ASCII.GetString(plainBytes));
        }
    }
}
