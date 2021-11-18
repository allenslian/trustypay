using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Xunit;

namespace TrustyPay.Core.Cryptography.Fixtures
{
    public class CertFixture
    {
        [Fact]
        public void ShouldReadCerFile()
        {
            var pubKey = RSAKeyFactory.ImportPublicKeyFromCerFile("./keys/self_cert.cer");
            Assert.True(pubKey.Key.Length > 0);
            Assert.True(pubKey.Format == RSACryptoProvider.PublicKeyFormat.X509);
        }

        [Fact]
        public void ShouldNotReadCerFile()
        {
            Assert.Throws<FileNotFoundException>(() =>
            {
                RSAKeyFactory.ImportPublicKeyFromCerFile("./keys/self_cert1.cer");
            });
        }

        [Fact]
        public void ShouldReadPfxFile()
        {
            var priKey = RSAKeyFactory.ImportPrivateKeyFromPfxFile("./keys/self_cert.pfx", "123456");
            Assert.True(priKey.Key.Length > 0);
            Assert.True(priKey.Format == RSACryptoProvider.PrivateKeyFormat.Pkcs8);
        }

        [Fact]
        public void ShouldNotReadPfxFile()
        {
            Assert.Throws<FileNotFoundException>(() =>
            {
                RSAKeyFactory.ImportPrivateKeyFromPfxFile("./keys/self_cert1.pfx", null);
            });

            Assert.ThrowsAny<CryptographicException>(() =>
            {
                RSAKeyFactory.ImportPrivateKeyFromPfxFile("./keys/self_cert.pfx", null);
            });
        }
    }

}