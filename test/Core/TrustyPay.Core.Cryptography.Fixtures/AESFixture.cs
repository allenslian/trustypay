using System;
using System.Text;
using System.Security.Cryptography;

using Xunit;

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
        public void ShouldFailToEncrypt()
        {
            var secretKey = Encoding.ASCII.GetBytes("a Secret Key!!!!!");
            var provider = new AESEncryptionProvider(secretKey, null);

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
            var provider = new AESEncryptionProvider(secretKey, null);

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

        [Fact]
        public void ShouldFailToEncryptWithBase64()
        {
            var secretKey = Encoding.ASCII.GetBytes("a Secret Key");
            IEncryptionProvider provider = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                var cipher = provider.EncryptToBase64String(null);
            });
            provider = new AESEncryptionProvider(secretKey, null);
            Assert.Throws<ArgumentNullException>(() =>
            {
                var cipher = provider.EncryptToBase64String(null);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                var cipher = provider.EncryptToBase64String(Array.Empty<byte>());
            });
        }

        [Fact]
        public void ShouldFailToDecryptWithBase64()
        {
            var secretKey = Encoding.ASCII.GetBytes("a Secret Key");
            IEncryptionProvider provider = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                var cipher = provider.DecryptFromBase64String(null);
            });
            provider = new AESEncryptionProvider(secretKey, null);
            Assert.Throws<ArgumentNullException>(() =>
            {
                var cipher = provider.DecryptFromBase64String(null);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                var cipher = provider.DecryptFromBase64String("");
            });
        }

        [Fact]
        public void ShouldEncryptAndDecryptWithHex()
        {
            var plainText = "hello, world!";
            var secretKey = Encoding.ASCII.GetBytes("a Secret Key");
            
            IEncryptionProvider provider = new AESEncryptionProvider(secretKey, null);
            var cipher = provider.EncryptToHexString(
                Encoding.UTF8.GetBytes(plainText));
            var plainBytes = provider.DecryptFromHexString(cipher);
            Assert.Equal(plainText, Encoding.UTF8.GetString(plainBytes));
        }

        [Fact]
        public void ShouldEncryptAndDecryptWithHex4()
        {
            var plainText = "hello, world!";
            var secretKey = Encoding.ASCII.GetBytes("a Secret Key");
            
            IEncryptionProvider provider = new AESEncryptionProvider(secretKey, null);
            var cipher = provider.EncryptToHexString(
                Encoding.UTF8.GetBytes(plainText), 4);
            var plainBytes = provider.DecryptFromHexString(cipher, 4);
            Assert.Equal(plainText, Encoding.UTF8.GetString(plainBytes));
        }

        [Fact]
        public void ShouldFailToEncryptWithHex()
        {
            var secretKey = Encoding.ASCII.GetBytes("a Secret Key");
            IEncryptionProvider provider = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                var cipher = provider.EncryptToHexString(null);
            });
            provider = new AESEncryptionProvider(secretKey, null);
            Assert.Throws<ArgumentNullException>(() =>
            {
                var cipher = provider.EncryptToHexString(null);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                var cipher = provider.EncryptToHexString(Array.Empty<byte>());
            });
            Assert.Throws<ArgumentException>(() =>
            {
                var cipher = provider.EncryptToHexString(new byte[]{114}, 0);
            });
        }

        [Fact]
        public void ShouldFailToDecryptWithHex()
        {
            var secretKey = Encoding.ASCII.GetBytes("a Secret Key");
            IEncryptionProvider provider = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                var cipher = provider.DecryptFromHexString(null);
            });
            provider = new AESEncryptionProvider(secretKey, null);
            Assert.Throws<ArgumentNullException>(() =>
            {
                var cipher = provider.DecryptFromHexString(null);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                var cipher = provider.DecryptFromHexString("");
            });
            Assert.Throws<ArgumentException>(() =>
            {
                var cipher = provider.DecryptFromHexString("E1", 0);
            });
            Assert.Throws<ArgumentException>(() =>
            {
                var cipher = provider.DecryptFromHexString("E1", 3);
            });
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
