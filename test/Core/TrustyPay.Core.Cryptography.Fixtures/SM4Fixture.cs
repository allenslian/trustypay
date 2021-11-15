using System;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace TrustyPay.Core.Cryptography.Fixtures
{
    public class SM4Fixture
    {
        [Theory]
        [InlineData(CipherMode.CBC)]
        [InlineData(CipherMode.ECB)]
        public void ShouldEncryptAndDecryptAsciiText(CipherMode mode)
        {
            var plainText = "hello";
            var secretKey = Encoding.ASCII.GetBytes("a Secret Key!!!!!");
            var provider = new SM4EncryptionProvider(secretKey, null, mode);
            var cipherBytes = provider.Encrypt(Encoding.ASCII.GetBytes(plainText));
            var plainBytes = provider.Decrypt(cipherBytes);
            Assert.Equal(plainText, Encoding.ASCII.GetString(plainBytes));
        }

        [Fact]
        public void ShouldFailToInitialize()
        {
            var secretKey = Encoding.ASCII.GetBytes("0000000000000000");
            Assert.Throws<ArgumentNullException>(() =>
            {
                new SM4EncryptionProvider(null, null, CipherMode.ECB, PaddingMode.None);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                new SM4EncryptionProvider(Array.Empty<byte>(), null, CipherMode.ECB, PaddingMode.ISO10126);
            });
        }

        [Fact]
        public void ShouldFailToEncrypt()
        {
            var plainText = "hello world";
            var secretKey = Encoding.ASCII.GetBytes("0000000000000000");
            var provider = new SM4EncryptionProvider(secretKey, null, CipherMode.ECB, PaddingMode.None);
            Assert.Throws<NotSupportedException>(() =>
            {
                provider.Encrypt(Encoding.UTF8.GetBytes(plainText));
            });

            provider = new SM4EncryptionProvider(secretKey, null, CipherMode.ECB, PaddingMode.ISO10126);
            Assert.Throws<NotSupportedException>(() =>
            {
                provider.Encrypt(Encoding.UTF8.GetBytes(plainText));
            });

            provider = new SM4EncryptionProvider(secretKey, null, CipherMode.CTS, PaddingMode.Zeros);
            Assert.Throws<NotSupportedException>(() =>
            {
                provider.Encrypt(Encoding.UTF8.GetBytes(plainText));
            });

            provider = new SM4EncryptionProvider(secretKey, null, CipherMode.ECB, PaddingMode.PKCS7);
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
            var plainText = "hello world";
            var secretKey = Encoding.ASCII.GetBytes("0000000000000000");
            var provider = new SM4EncryptionProvider(secretKey, null, CipherMode.ECB, PaddingMode.None);
            Assert.Throws<NotSupportedException>(() =>
            {
                provider.Decrypt(Encoding.UTF8.GetBytes(plainText));
            });

            provider = new SM4EncryptionProvider(secretKey, null, CipherMode.CTS, PaddingMode.Zeros);
            Assert.Throws<NotSupportedException>(() =>
            {
                provider.Decrypt(Encoding.UTF8.GetBytes(plainText));
            });

            provider = new SM4EncryptionProvider(secretKey, null, CipherMode.ECB, PaddingMode.PKCS7);
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
        public void ShouldEncryptAsciiTextWithECB()
        {
            var plainText = "hello world";
            var secretKey = Encoding.ASCII.GetBytes("0000000000000000");
            var provider = new SM4EncryptionProvider(secretKey, null, CipherMode.ECB, PaddingMode.PKCS7);
            var cipherText = provider.EncryptToBase64String(Encoding.UTF8.GetBytes(plainText));
            Assert.Equal("qom/iKcNYQ49nPg5+sfsyw==", cipherText);
        }

        [Fact]
        public void ShouldEncryptAsciiTextWithECBAndPadding()
        {
            var plainText = "hello world";
            var secretKey = Encoding.ASCII.GetBytes("0000000000000000");
            var provider = new SM4EncryptionProvider(secretKey, null, CipherMode.ECB, PaddingMode.Zeros);
            var cipherText = provider.EncryptToBase64String(Encoding.UTF8.GetBytes(plainText));
            Assert.Equal("9jNMLFon8o3WqXTXTODnRw==", cipherText);

            provider = new SM4EncryptionProvider(secretKey, null, CipherMode.ECB, PaddingMode.ANSIX923);
            cipherText = provider.EncryptToBase64String(Encoding.UTF8.GetBytes(plainText));
            Assert.Equal("s+xdbI17EE4obly4jdW7ew==", cipherText);
        }

        [Fact]
        public void ShouldDecryptAsciiTextWithECB()
        {
            var plainText = "hello world";
            var secretKey = Encoding.ASCII.GetBytes("0000000000000000");
            var cipherBytes = Convert.FromBase64String("qom/iKcNYQ49nPg5+sfsyw==");
            var provider = new SM4EncryptionProvider(secretKey, null, CipherMode.ECB, PaddingMode.PKCS7);
            var plainBytes = provider.Decrypt(cipherBytes);
            Assert.Equal(plainText, Encoding.ASCII.GetString(plainBytes));
        }

        [Fact]
        public void ShouldEncryptAsciiTextWithCBC()
        {
            var plainText = "hello world";
            var secretKey = Encoding.UTF8.GetBytes("0000000000000000");
            var provider = new SM4EncryptionProvider(
                secretKey,
                Encoding.UTF8.GetBytes("8a6c4ddd8a6c4ddd"),
                CipherMode.CBC,
                PaddingMode.PKCS7);
            var cipherBytes = provider.Encrypt(Encoding.UTF8.GetBytes(plainText));
            Assert.Equal("hwWIPRHNn1d01yv5b+6omw==", Convert.ToBase64String(cipherBytes));

            provider = new SM4EncryptionProvider(
                secretKey,
                Encoding.UTF8.GetBytes("8a6c4ddd8a6c4ddd"),
                CipherMode.CBC,
                PaddingMode.Zeros);
            cipherBytes = provider.Encrypt(Encoding.UTF8.GetBytes(plainText));
            Assert.Equal("ALcK5jap9quJPber4nSjfg==", Convert.ToBase64String(cipherBytes));

            provider = new SM4EncryptionProvider(
                secretKey,
                Encoding.UTF8.GetBytes("8a6c4ddd8a6c4ddd"),
                CipherMode.CBC,
                PaddingMode.ANSIX923);
            cipherBytes = provider.Encrypt(Encoding.UTF8.GetBytes(plainText));
            Assert.Equal("n72lSziHwEM2Ln8XGV5YfQ==", Convert.ToBase64String(cipherBytes));
        }

        [Fact]
        public void ShouldDecryptAsciiTextWithCBC()
        {
            var plainText = "hello world";
            var secretKey = Encoding.ASCII.GetBytes("0000000000000000");
            var cipherBytes = Convert.FromBase64String("hwWIPRHNn1d01yv5b+6omw==");
            var provider = new SM4EncryptionProvider(
                secretKey,
                Encoding.ASCII.GetBytes("8a6c4ddd8a6c4ddd"),
                CipherMode.CBC,
                PaddingMode.PKCS7);
            var plainBytes = provider.Decrypt(cipherBytes);
            Assert.Equal(plainText, Encoding.ASCII.GetString(plainBytes));
        }
    }
}