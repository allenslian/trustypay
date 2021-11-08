using System;
using System.Text;
using Xunit;
using System.Security.Cryptography;

namespace TrustyPay.Core.Cryptography.Fixtures
{
    public class AESFixture
    {
        [Fact]
        public void TestInitializeAESEncryptionProviderWithoutSecretKey()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var provider = new AESEncryptionProvider(null, null);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                var provider = new AESEncryptionProvider(Array.Empty<byte>(), null);
            });
        }

        [Fact]
        public void ShouldEncryptAndDecryptAsciiText()
        {
            var plainText = "hello";
            var secretKey = Encoding.ASCII.GetBytes("a Secret Key!!!!!");
            var provider = new AESEncryptionProvider(secretKey, null);
            var cipherBytes = provider.Encrypt(Encoding.ASCII.GetBytes(plainText));
            var plainBytes = provider.Decrypt(cipherBytes);
            Assert.Equal(plainText, Encoding.ASCII.GetString(plainBytes));
        }

        [Fact]
        public void ShouldEncryptAndDecryptUTF8Text()
        {
            var plainText = "hello, 世界";
            var secretKey = Encoding.ASCII.GetBytes("a Secret Key");
            var provider = new AESEncryptionProvider(secretKey, null);
            var cipherBytes = provider.Encrypt(Encoding.UTF8.GetBytes(plainText));
            var plainBytes = provider.Decrypt(cipherBytes);
            Assert.Equal(plainText, Encoding.UTF8.GetString(plainBytes));
        }

        [Fact]
        public void ShouldEncryptAndDecryptGBKText()
        {
            var plainText = "hello, 世界";
            var secretKey = Encoding.ASCII.GetBytes("a Secret Key");
            
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var provider = new AESEncryptionProvider(secretKey, null);
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
            IEncryptionProvider provider = new AESEncryptionProvider(secretKey, null);
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
            var provider = new AESEncryptionProvider(secretKey, null, 
                AESEncryptionProvider.KeySizes.AES128, mode);
            var cipherBytes = provider.Encrypt(Encoding.ASCII.GetBytes(plainText));
            var plainBytes = provider.Decrypt(cipherBytes);
            Assert.Equal(plainText, Encoding.ASCII.GetString(plainBytes));
        }
    }
}
