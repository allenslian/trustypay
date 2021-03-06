using System;
using System.Text;
using Xunit;
using System.Security.Cryptography;

namespace TrustyPay.Core.Cryptography.Fixtures
{
    public class TripleDESFixture
    {
        [Fact]
        public void TestInitializeTripleDESEncryptionProviderWithoutSecretKey()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var provider = new TripleDESEncryptionProvider(null, null);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                var provider = new TripleDESEncryptionProvider(Array.Empty<byte>(), null);
            });
        }

        [Fact]
        public void ShouldFailToEncrypt()
        {
            var secretKey = Encoding.ASCII.GetBytes("a Secret Key!!!!!");
            var provider = new TripleDESEncryptionProvider(secretKey, null);

            Assert.Throws<ArgumentNullException>(() =>
            {
                provider.Encrypt(null);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                provider.Encrypt(Array.Empty<byte>());
            });
        }

        [Fact]
        public void ShouldFailToDecrypt()
        {
            var secretKey = Encoding.ASCII.GetBytes("a Secret Key!!!!!");
            var provider = new TripleDESEncryptionProvider(secretKey, null);

            Assert.Throws<ArgumentNullException>(() =>
            {
                provider.Decrypt(null);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                provider.Decrypt(Array.Empty<byte>());
            });
        }

        [Fact]
        public void ShouldEncryptAndDecryptAsciiText()
        {
            var plainText = "hello";
            var secretKey = Encoding.ASCII.GetBytes("a Secret Key!!!!!");
            var provider = new TripleDESEncryptionProvider(secretKey, null);
            var cipherBytes = provider.Encrypt(Encoding.ASCII.GetBytes(plainText));
            var plainBytes = provider.Decrypt(cipherBytes);
            Assert.Equal(plainText, Encoding.ASCII.GetString(plainBytes));
        }

        [Fact]
        public void ShouldEncryptAndDecryptUTF8Text()
        {
            var plainText = "hello, ??????";
            var secretKey = Encoding.ASCII.GetBytes("a Secret Key");
            var provider = new TripleDESEncryptionProvider(secretKey, null);
            var cipherBytes = provider.Encrypt(Encoding.UTF8.GetBytes(plainText));
            var plainBytes = provider.Decrypt(cipherBytes);
            Assert.Equal(plainText, Encoding.UTF8.GetString(plainBytes));
        }

        [Fact]
        public void ShouldEncryptAndDecryptGBKText()
        {
            var plainText = "hello, ??????";
            var secretKey = Encoding.ASCII.GetBytes("123456781234567812345678");
            var iv = Encoding.ASCII.GetBytes("12345678");
            
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var provider = new TripleDESEncryptionProvider(secretKey, iv);
            var cipherBytes = provider.Encrypt(Encoding.GetEncoding("GBK").GetBytes(plainText));
            var plainBytes = provider.Decrypt(cipherBytes);
            Assert.Equal(plainText, Encoding.GetEncoding("GBK").GetString(plainBytes));
        }

        [Fact]
        public void ShouldEncryptAndDecryptWithBase64()
        {
            var plainText = "hello, ??????";
            var secretKey = Encoding.ASCII.GetBytes("a Secret Key");
            
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var provider = new TripleDESEncryptionProvider(secretKey, null);
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
            var provider = new TripleDESEncryptionProvider(secretKey, null, mode);
            var cipherBytes = provider.Encrypt(Encoding.ASCII.GetBytes(plainText));
            var plainBytes = provider.Decrypt(cipherBytes);
            Assert.Equal(plainText, Encoding.ASCII.GetString(plainBytes));
        }
    }
}
