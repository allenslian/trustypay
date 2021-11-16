
using System;
using System.Text;
using Xunit;

namespace TrustyPay.Core.Cryptography.Fixtures
{
    public class DigestFixture
    {
        [Fact]
        public void ShouldBeMD5()
        {
            var provider = new MD5DigestProvider();
            var hash = provider.Hash(Encoding.ASCII.GetBytes("hello world"));
            Assert.Equal("XrY7u+Ae7tCTyyK7j1rNww==", hash.ToBase64String());
        }

        [Fact]
        public void ShouldFailToHashWithMD5()
        {
            var provider = new MD5DigestProvider();
            Assert.Throws<ArgumentNullException>(() =>
            {
                provider.Hash(null);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                provider.Hash(Array.Empty<byte>());
            });
        }

        [Fact]
        public void ShouldBeSHA256()
        {
            var provider = new SHA256DigestProvider();
            var hash = provider.Hash(Encoding.ASCII.GetBytes("hello world"));
            Assert.Equal("uU0nuZNNPgilLlLX2n2r+sSE7+N6U4DukIj3rOLvzek=", hash.ToBase64String());
        }

        [Fact]
        public void ShouldFailToHashWithSHA256()
        {
            var provider = new SHA256DigestProvider();
            Assert.Throws<ArgumentNullException>(() =>
            {
                provider.Hash(null);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                provider.Hash(Array.Empty<byte>());
            });
        }

        [Fact]
        public void ShouldBePBKDF2WithSHA1()
        {
            var provider = new PBKDF2DigestProvider(
                Encoding.UTF8.GetBytes("000000000"),
                System.Security.Cryptography.HashAlgorithmName.SHA1
            );
            var hash = provider.Hash(Encoding.ASCII.GetBytes("hello world"));
            Assert.Equal("M2RyV6COwWY+4cJRXlxkoQ==", hash.ToBase64String());
        }

        [Fact]
        public void ShouldFailToHashWithPBKDF2()
        {
            var provider = new PBKDF2DigestProvider(
                Encoding.UTF8.GetBytes("00000000"),
                System.Security.Cryptography.HashAlgorithmName.SHA1
            );
            Assert.Throws<ArgumentNullException>(() =>
            {
                provider.Hash(null);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                provider.Hash(Array.Empty<byte>());
            });
        }

        [Fact]
        public void ShouldBePBKDF2WithSHA256()
        {
            var provider = new PBKDF2DigestProvider(
                Encoding.UTF8.GetBytes("000000000"),
                System.Security.Cryptography.HashAlgorithmName.SHA256
            );
            var hash = provider.Hash(Encoding.ASCII.GetBytes("hello world"));
            Assert.Equal("rUX0OAFSyXCUfgsQayoN/A==", hash.ToBase64String());
        }

        [Fact]
        public void ShouldBePBKDF2WithSHA384()
        {
            var provider = new PBKDF2DigestProvider(
                Encoding.UTF8.GetBytes("000000000"),
                System.Security.Cryptography.HashAlgorithmName.SHA384
            );
            var hash = provider.Hash(Encoding.ASCII.GetBytes("hello world"));
            Assert.Equal("Bsx6mUzEcnA1jWE+gd08LA==", hash.ToBase64String());
        }

        [Fact]
        public void ShouldBePBKDF2WithSHA512()
        {
            var provider = new PBKDF2DigestProvider(
                Encoding.UTF8.GetBytes("000000000"),
                System.Security.Cryptography.HashAlgorithmName.SHA512
            );
            var hash = provider.Hash(Encoding.ASCII.GetBytes("hello world"));
            Assert.Equal("H+GFNNE6Oh0iPhjvuW2INQ==", hash.ToBase64String());
        }

        [Fact]
        public void ShouldFailToPBKDF2()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new PBKDF2DigestProvider(
                    null,
                    System.Security.Cryptography.HashAlgorithmName.SHA1
                );
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                new PBKDF2DigestProvider(
                    Array.Empty<byte>(),
                    System.Security.Cryptography.HashAlgorithmName.SHA1
                );
            });
            Assert.Throws<ArgumentException>(() =>
            {
                new PBKDF2DigestProvider(
                    Encoding.UTF8.GetBytes("000000"),
                    System.Security.Cryptography.HashAlgorithmName.SHA1
                );
            });

            Assert.Throws<ArgumentException>(() =>
            {
                new PBKDF2DigestProvider(
                    Encoding.UTF8.GetBytes("00000000"),
                    System.Security.Cryptography.HashAlgorithmName.MD5
                );
            });

            Assert.Throws<ArgumentException>(() =>
            {
                new PBKDF2DigestProvider(
                    Encoding.UTF8.GetBytes("00000000"),
                    System.Security.Cryptography.HashAlgorithmName.SHA1,
                    0
                );
            });
            Assert.Throws<ArgumentException>(() =>
            {
                new PBKDF2DigestProvider(
                    Encoding.UTF8.GetBytes("00000000"),
                    System.Security.Cryptography.HashAlgorithmName.SHA1,
                    1000,
                    0
                );
            });
        }

        
    }

}