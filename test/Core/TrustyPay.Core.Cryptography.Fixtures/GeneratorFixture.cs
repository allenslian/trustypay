using System;
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
        }

        [Fact]
        public void ShouldGenerateAlphabetString()
        {
            var alphabets = SecretKeyGenerator.GenerateRandomAlphabets(12);
            Assert.NotNull(alphabets);
            Assert.True(alphabets.Length == 12);
        }

        [Fact]
        public void ShouldGenerateAlphabetAndNumbersString()
        {
            var rand = SecretKeyGenerator.GenerateRandomAlphabetAndNumbers(16);
            Assert.NotNull(rand);
            Assert.True(rand.Length == 16);
        }
    }
}