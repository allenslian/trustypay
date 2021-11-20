using System;
using System.Linq;
using Xunit;

namespace TrustyPay.Core.Cryptography.Fixtures
{
    public class GeneratorFixture
    {
        [Fact]
        public void ShouldGenerateSalt()
        {
            var salt = SecretKeyGenerator.GenerateRandomSalt(0);
            Assert.Empty(salt);

            salt = SecretKeyGenerator.GenerateRandomSalt(12);
            Assert.NotEmpty(salt);
            Assert.True(salt.Length == 12);
        }

        [Fact]
        public void ShouldFailToGenerateSalt()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                SecretKeyGenerator.GenerateRandomSalt(-1);
            });
        }

        [Fact]
        public void ShouldGenerateHexString()
        {
            var hex = SecretKeyGenerator.GenerateRandomHexString(8);
            Assert.NotNull(hex);
            Assert.True(hex.Length == 8);
        }

        [Fact]
        public void ShouldFailToGenerateHexString()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                SecretKeyGenerator.GenerateRandomHexString(11);
            });
        }

        [Fact]
        public void ShouldGenerateBase64String()
        {
            var base64 = SecretKeyGenerator.GenerateRandomBase64String(12);
            Assert.NotNull(base64);
            Assert.True(base64.Length == 12);
        }

        [Fact]
        public void ShouldGenerateNumberString()
        {
            var numbers = SecretKeyGenerator.GenerateRandomNumbers(8);
            Assert.NotNull(numbers);
            Assert.True(numbers.Length == 8);

            var allNumbers = numbers.All(c => char.IsNumber(c));
            Assert.True(allNumbers);
        }

        [Fact]
        public void ShouldGenerateAlphabetString()
        {
            var alphabets = SecretKeyGenerator.GenerateRandomAlphabets(12);
            Assert.NotNull(alphabets);
            Assert.True(alphabets.Length == 12);

            var allLetters = alphabets.All(c => char.IsLetter(c));
            Assert.True(allLetters);
        }

        [Fact]
        public void ShouldGenerateAlphabetAndNumbersString()
        {
            var chars = SecretKeyGenerator.GenerateRandomAlphabetAndNumbers(16);
            Assert.NotNull(chars);
            Assert.True(chars.Length == 16);

            var allLetterAndNumbers = chars.All(c => char.IsLetterOrDigit(c));
            Assert.True(allLetterAndNumbers);
        }

        [Fact]
        public void ShouldFailToGenerateRSAPkcs1KeyPair()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                RSAKeyGenerator.GenerateKeyPair(100);
            });
            Assert.Throws<NotSupportedException>(() =>
            {
                RSAKeyGenerator.GenerateKeyPair(1024, (RSACryptoProvider.PrivateKeyFormat)100);
            });
        }

        [Fact]
        public void ShouldGenerateRSAPkcs1KeyPair()
        {
            var pairs = RSAKeyGenerator.GenerateKeyPair();
            Assert.NotNull(pairs);
            Assert.True(pairs.Item1.Key.Length > 0);
            Assert.True(pairs.Item1.Format == RSACryptoProvider.PrivateKeyFormat.Pkcs1);
            Assert.True(pairs.Item2.Key.Length > 0);
            Assert.True(pairs.Item2.Format == RSACryptoProvider.PublicKeyFormat.Pkcs1);
        }

        [Fact]
        public void ShouldGenerateRSAPkcs8KeyPair()
        {
            var pairs = RSAKeyGenerator.GenerateKeyPair(2048, RSACryptoProvider.PrivateKeyFormat.Pkcs8);
            Assert.NotNull(pairs);
            Assert.True(pairs.Item1.Key.Length > 0);
            Assert.True(pairs.Item1.Format == RSACryptoProvider.PrivateKeyFormat.Pkcs8);
            Assert.True(pairs.Item2.Key.Length > 0);
            Assert.True(pairs.Item2.Format == RSACryptoProvider.PublicKeyFormat.X509);
        }
    }
}