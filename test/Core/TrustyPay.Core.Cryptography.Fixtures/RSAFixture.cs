using System;
using Xunit;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TrustyPay.Core.Cryptography.Fixtures
{
    public class RSAFixture
    {
        [Fact]
        public async void ShouldReadPemFile()
        {
            var priKey = await RSAKeyFactory.ImportPrivateKeyFromPemFileAsync("./keys/self_pkcs1.key");
            Assert.True(priKey.Format == RSACryptoProvider.PrivateKeyFormat.Pkcs1);
            Assert.NotNull(priKey.Key);
            Assert.True(priKey.Key.Length > 0);

            priKey = await RSAKeyFactory.ImportPrivateKeyFromPemFileAsync("./keys/self_pkcs8.key");
            Assert.True(priKey.Format == RSACryptoProvider.PrivateKeyFormat.Pkcs8);
            Assert.NotNull(priKey.Key);
            Assert.True(priKey.Key.Length > 0);

            var pubKey = await RSAKeyFactory.ImportPublicKeyFromPemFileAsync("./keys/self_pkcs1.pub");
            Assert.True(pubKey.Format == RSACryptoProvider.PublicKeyFormat.Pkcs1);
            Assert.NotNull(pubKey.Key);
            Assert.True(pubKey.Key.Length > 0);

            pubKey = await RSAKeyFactory.ImportPublicKeyFromPemFileAsync("./keys/self_x509.pub");
            Assert.True(pubKey.Format == RSACryptoProvider.PublicKeyFormat.X509);
            Assert.NotNull(pubKey.Key);
            Assert.True(pubKey.Key.Length > 0);
        }

        [Fact]
        public async void ShouldFailToReadPemFile()
        {
            await Assert.ThrowsAsync<FileNotFoundException>(async () =>
            {
                var priKey = await RSAKeyFactory.ImportPrivateKeyFromPemFileAsync("./keys/self_pkcs2.key");
            });

            await Assert.ThrowsAsync<FileNotFoundException>(async () =>
            {
                var priKey = await RSAKeyFactory.ImportPublicKeyFromPemFileAsync("./keys/self_pkcs2.pub");
            });
        }

        [Fact]
        public void ShouldFailToReadPemText()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var priKey = RSAKeyFactory.ImportPrivateKeyFromPemText(null);
            });

            Assert.Throws<InvalidDataException>(() =>
            {
                var priKey = RSAKeyFactory.ImportPrivateKeyFromPemText("./keys/self_pkcs2.key");
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                var priKey = RSAKeyFactory.ImportPublicKeyFromPemText(null);
            });

            Assert.Throws<InvalidDataException>(() =>
            {
                var priKey = RSAKeyFactory.ImportPublicKeyFromPemText("./keys/self_pkcs2.key");
            });
        }

        [Fact]
        public void ShouldReadPemText()
        {
            var priKey = RSAKeyFactory.ImportPrivateKeyFromPemText(
              @"-----BEGIN RSA PRIVATE KEY-----
MIIEowIBAAKCAQEA3oCRWf56TdNN0eln8XxCQP6BiMQdx1xOrqqLK3LJ7uvRZmNO
4GHnWWr6DBLpUmVNHWzoNCaWJZh8TGKnJHD32BTvTLNmgWAdSqr0cq7sPMnNmhOH
kuAOiuTaLjA5CJf/4JLkRZcNJvyIyK8hpDBoFWrCeB/ToQSRaLWByptfhL361/tW
kwUv/4GSlAyo97j+fVmJ4KNzf/7vNmc3USIS1UpDPqp8vtxdOHDd+mj/Xe/l7sJu
iU8ntJniUn0WACoLIOYD34B4t9eTaXMudR2uWbpG0DfF892sSELogcSfCtuwfmV4
Z88CD3cxJkDl4SMbREeXxpfUNmwGdk6k6ySpEwIDAQABAoIBAQDSK92ghWlftBYR
DwlZKMeofv86gg2i8AV+paeZ3e7y2RvOPEYYW0Jdy1fwKbjtVLT385SPFlfDoXrf
6Fv7Zx4lpPi5mchcsr5Ydo06+xI5BWJJhOMOEMiL2EpjULe+710K85XeSiVpSyel
cRA0+GiIunSBZYbQ1PDJWJIGIKekSNDV8gR8iml9gqrxsgCSqMtd6lemcZWWO3Sp
DvcgXOPOgj5dvs1wzicbKd27b38iMElBwX3UUgwolKAsIh73BUYyodM5UZccaedz
Wyfk3QfVrn22rFkYMNTtXcz++QOoMhnMdh/erKL37rvn1/6Mr52Rh3oIMgySsb1B
vfyfhFwpAoGBAPam1e3kSLK8F6WZn0NGE4EVzo2PAF4R1hn0r48fqNmUeqPZg/Dn
Sd/16Gx/M8piPndqTwIHjXqUhJgGIBO8rMwi4FsV8+AUiiRI/AlZMyxlF5/3Xzv8
PWP0afx/zlT7F0Xsui/HrleDmO4/v1k8X7Xl7hkebeZmermsVZk49ca/AoGBAObv
a0+4G4Hut3U1S2Y2ViT5laxXfNTYShggrFiT6WpeBvZwx03GpMnmPWyK2um4hDSZ
86JodPWFR9mrG4dtEcNGW54hPp4yITkyRy+hHMllMdIaMmOY3DyS8RmTg7yrF7tI
8u12TfVOrarS8BpLMMRj9Pt3wkseohqM7f2GsyatAoGAbKZfVFScJYevjvYV7ud+
jf1SKI9WpRmMS5C6iPx0P6wlPeoCMetgYnSLdsetw7f1NlsxBH7ZNqcXpXQFS0xw
fdfbJqSL8ih6FbPEukV9wk+h0YiBfgYF8PLogR63gD2/KcE23vdB+DDy+/g+zQtI
SfNIJ+58kOKRqxqb9kveEDcCgYBYeVlPjWVcfixjByv/2MlgGQ6ynEIC+WpJnBrr
RJ+kVKmuOL7imTwA7eiScRA1gq+Dx4eDrSlB9vHz/o3pcGvhuE7ZYjsvOF1qIE0N
flgdAFv59ndfmOyneFROTCmoWpQY+HW5bB2p4Z6/V1kNeckRNIpi3Rre1LmeGmgD
PVvdcQKBgF5WJbEebU+ACEqTyw+DPytu8uWJp+kKUxILbprAB0YrIbI2wvznLd1t
gPBBr6nISCv8td6F6/Wt4+gs57PYq9eAfzjelyhwA/0ft1UPdXpKZ2XOrkreGQLl
zOxMcDleX+fgV0vUlURuSbQtEGkqkZeJ28VPs+QmbEEuo2Zh+fzq
-----END RSA PRIVATE KEY-----");
            Assert.True(priKey.Format == RSACryptoProvider.PrivateKeyFormat.Pkcs1);
            Assert.NotNull(priKey.Key);
            Assert.True(priKey.Key.Length > 0);

            priKey = RSAKeyFactory.ImportPrivateKeyFromPemText(
              @"-----BEGIN PRIVATE KEY-----
MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDegJFZ/npN003R
6WfxfEJA/oGIxB3HXE6uqosrcsnu69FmY07gYedZavoMEulSZU0dbOg0JpYlmHxM
YqckcPfYFO9Ms2aBYB1KqvRyruw8yc2aE4eS4A6K5NouMDkIl//gkuRFlw0m/IjI
ryGkMGgVasJ4H9OhBJFotYHKm1+EvfrX+1aTBS//gZKUDKj3uP59WYngo3N//u82
ZzdRIhLVSkM+qny+3F04cN36aP9d7+Xuwm6JTye0meJSfRYAKgsg5gPfgHi315Np
cy51Ha5ZukbQN8Xz3axIQuiBxJ8K27B+ZXhnzwIPdzEmQOXhIxtER5fGl9Q2bAZ2
TqTrJKkTAgMBAAECggEBANIr3aCFaV+0FhEPCVkox6h+/zqCDaLwBX6lp5nd7vLZ
G848RhhbQl3LV/ApuO1UtPfzlI8WV8Ohet/oW/tnHiWk+LmZyFyyvlh2jTr7EjkF
YkmE4w4QyIvYSmNQt77vXQrzld5KJWlLJ6VxEDT4aIi6dIFlhtDU8MlYkgYgp6RI
0NXyBHyKaX2CqvGyAJKoy13qV6ZxlZY7dKkO9yBc486CPl2+zXDOJxsp3btvfyIw
SUHBfdRSDCiUoCwiHvcFRjKh0zlRlxxp53NbJ+TdB9WufbasWRgw1O1dzP75A6gy
Gcx2H96sovfuu+fX/oyvnZGHeggyDJKxvUG9/J+EXCkCgYEA9qbV7eRIsrwXpZmf
Q0YTgRXOjY8AXhHWGfSvjx+o2ZR6o9mD8OdJ3/XobH8zymI+d2pPAgeNepSEmAYg
E7yszCLgWxXz4BSKJEj8CVkzLGUXn/dfO/w9Y/Rp/H/OVPsXRey6L8euV4OY7j+/
WTxfteXuGR5t5mZ6uaxVmTj1xr8CgYEA5u9rT7gbge63dTVLZjZWJPmVrFd81NhK
GCCsWJPpal4G9nDHTcakyeY9bIra6biENJnzomh09YVH2asbh20Rw0ZbniE+njIh
OTJHL6EcyWUx0hoyY5jcPJLxGZODvKsXu0jy7XZN9U6tqtLwGkswxGP0+3fCSx6i
Gozt/YazJq0CgYBspl9UVJwlh6+O9hXu536N/VIoj1alGYxLkLqI/HQ/rCU96gIx
62BidIt2x63Dt/U2WzEEftk2pxeldAVLTHB919smpIvyKHoVs8S6RX3CT6HRiIF+
BgXw8uiBHreAPb8pwTbe90H4MPL7+D7NC0hJ80gn7nyQ4pGrGpv2S94QNwKBgFh5
WU+NZVx+LGMHK//YyWAZDrKcQgL5akmcGutEn6RUqa44vuKZPADt6JJxEDWCr4PH
h4OtKUH28fP+jelwa+G4TtliOy84XWogTQ1+WB0AW/n2d1+Y7Kd4VE5MKahalBj4
dblsHanhnr9XWQ15yRE0imLdGt7UuZ4aaAM9W91xAoGAXlYlsR5tT4AISpPLD4M/
K27y5Ymn6QpTEgtumsAHRishsjbC/Oct3W2A8EGvqchIK/y13oXr9a3j6Czns9ir
14B/ON6XKHAD/R+3VQ91ekpnZc6uSt4ZAuXM7ExwOV5f5+BXS9SVRG5JtC0QaSqR
l4nbxU+z5CZsQS6jZmH5/Oo=
-----END PRIVATE KEY-----");
            Assert.True(priKey.Format == RSACryptoProvider.PrivateKeyFormat.Pkcs8);
            Assert.NotNull(priKey.Key);
            Assert.True(priKey.Key.Length > 0);

            var pubKey = RSAKeyFactory.ImportPublicKeyFromPemText(
              @"-----BEGIN RSA PUBLIC KEY-----
MIIBCgKCAQEA3oCRWf56TdNN0eln8XxCQP6BiMQdx1xOrqqLK3LJ7uvRZmNO4GHn
WWr6DBLpUmVNHWzoNCaWJZh8TGKnJHD32BTvTLNmgWAdSqr0cq7sPMnNmhOHkuAO
iuTaLjA5CJf/4JLkRZcNJvyIyK8hpDBoFWrCeB/ToQSRaLWByptfhL361/tWkwUv
/4GSlAyo97j+fVmJ4KNzf/7vNmc3USIS1UpDPqp8vtxdOHDd+mj/Xe/l7sJuiU8n
tJniUn0WACoLIOYD34B4t9eTaXMudR2uWbpG0DfF892sSELogcSfCtuwfmV4Z88C
D3cxJkDl4SMbREeXxpfUNmwGdk6k6ySpEwIDAQAB
-----END RSA PUBLIC KEY-----");
            Assert.True(pubKey.Format == RSACryptoProvider.PublicKeyFormat.Pkcs1);
            Assert.NotNull(pubKey.Key);
            Assert.True(pubKey.Key.Length > 0);

            pubKey = RSAKeyFactory.ImportPublicKeyFromPemText(
              @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA3oCRWf56TdNN0eln8XxC
QP6BiMQdx1xOrqqLK3LJ7uvRZmNO4GHnWWr6DBLpUmVNHWzoNCaWJZh8TGKnJHD3
2BTvTLNmgWAdSqr0cq7sPMnNmhOHkuAOiuTaLjA5CJf/4JLkRZcNJvyIyK8hpDBo
FWrCeB/ToQSRaLWByptfhL361/tWkwUv/4GSlAyo97j+fVmJ4KNzf/7vNmc3USIS
1UpDPqp8vtxdOHDd+mj/Xe/l7sJuiU8ntJniUn0WACoLIOYD34B4t9eTaXMudR2u
WbpG0DfF892sSELogcSfCtuwfmV4Z88CD3cxJkDl4SMbREeXxpfUNmwGdk6k6ySp
EwIDAQAB
-----END PUBLIC KEY-----");
            Assert.True(pubKey.Format == RSACryptoProvider.PublicKeyFormat.X509);
            Assert.NotNull(pubKey.Key);
            Assert.True(pubKey.Key.Length > 0);
        }

        [Fact]
        public async void ShouldSignAndVerifyWithPkcs8()
        {
            var pri = await RSAKeyFactory.ImportPrivateKeyFromPemFileAsync("./keys/self_pkcs8.key");
            var pub = await RSAKeyFactory.ImportPublicKeyFromPemFileAsync("./keys/self_x509.pub");
            var plainBytes = Encoding.ASCII.GetBytes("hello world!");
            var provider = new RSACryptoProvider(pri, pub, RSASignaturePadding.Pkcs1);
            var signature = provider.Sign(plainBytes, HashAlgorithmName.SHA256);
            Assert.True(provider.Verify(plainBytes, signature, HashAlgorithmName.SHA256));
        }

        [Fact]
        public async void ShouldSignAndVerifyWithPkcs1()
        {
            // await GeneratePkcs1PublicKeyAsync();
            var pri = await RSAKeyFactory.ImportPrivateKeyFromPemFileAsync("./keys/self_pkcs1.key");
            var pub = await RSAKeyFactory.ImportPublicKeyFromPemFileAsync("./keys/self_pkcs1.pub");
            var plainBytes = Encoding.ASCII.GetBytes("hello world!");
            var provider = new RSACryptoProvider(pri, pub, RSASignaturePadding.Pkcs1);
            var signature = provider.Sign(plainBytes, HashAlgorithmName.SHA256);
            Assert.True(provider.Verify(plainBytes, signature, HashAlgorithmName.SHA256));
        }

        [Fact]
        public async void ShouldEncryptAndDecryptWithPkcs1()
        {
            var pri = await RSAKeyFactory.ImportPrivateKeyFromPemFileAsync("./keys/self_pkcs1.key");
            var pub = await RSAKeyFactory.ImportPublicKeyFromPemFileAsync("./keys/self_pkcs1.pub");
            var plainBytes = Encoding.ASCII.GetBytes("hello world!");
            var provider = new RSACryptoProvider(pri, pub, RSAEncryptionPadding.Pkcs1);
            var cipherBytes = provider.Encrypt(plainBytes);
            plainBytes = provider.Decrypt(cipherBytes);
            Assert.Equal("hello world!", Encoding.ASCII.GetString(plainBytes));
        }

        [Fact]
        public async void ShouldFailToEncrypt()
        {
            var pri = await RSAKeyFactory.ImportPrivateKeyFromPemFileAsync("./keys/self_pkcs1.key");
            var pub = await RSAKeyFactory.ImportPublicKeyFromPemFileAsync("./keys/self_pkcs1.pub");
            var provider = new RSACryptoProvider(pri, pub, RSAEncryptionPadding.Pkcs1);
            Assert.Throws<ArgumentNullException>(() =>
            {
                var cipherBytes = provider.Encrypt(null);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                var cipherBytes = provider.Encrypt(Array.Empty<byte>());
            });
        }

        [Fact]
        public async void ShouldFailToDecrypt()
        {
            var pri = await RSAKeyFactory.ImportPrivateKeyFromPemFileAsync("./keys/self_pkcs1.key");
            var pub = await RSAKeyFactory.ImportPublicKeyFromPemFileAsync("./keys/self_pkcs1.pub");
            var provider = new RSACryptoProvider(pri, pub, RSAEncryptionPadding.Pkcs1);
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
        public async void ShouldEncryptAndDecryptWithPkcs8()
        {
            var pri = await RSAKeyFactory.ImportPrivateKeyFromPemFileAsync("./keys/self_pkcs8.key");
            var pub = await RSAKeyFactory.ImportPublicKeyFromPemFileAsync("./keys/self_x509.pub");
            var plainBytes = Encoding.ASCII.GetBytes("hello world!");
            var provider = new RSACryptoProvider(pri, pub, RSAEncryptionPadding.Pkcs1);
            var cipherBytes = provider.Encrypt(plainBytes);
            plainBytes = provider.Decrypt(cipherBytes);
            Assert.Equal("hello world!", Encoding.ASCII.GetString(plainBytes));
        }

        [Fact]
        public async void ShouldFailToSignBase64WithPkcs1()
        {
            var plainBytes = Encoding.UTF8.GetBytes("hello, 世界");
            var pri = await RSAKeyFactory.ImportPrivateKeyFromPemFileAsync("./keys/self_pkcs1.key");
            var pub = await RSAKeyFactory.ImportPublicKeyFromPemFileAsync("./keys/self_pkcs1.pub");
            ISignatureProvider provider = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                provider.SignToBase64String(null, HashAlgorithmName.SHA256);
            });

            provider = new RSACryptoProvider(pri, pub, RSASignaturePadding.Pkcs1);
            Assert.Throws<ArgumentNullException>(() =>
            {
                provider.SignToBase64String(Array.Empty<byte>(), HashAlgorithmName.SHA256);
            });
        }

        [Fact]
        public async void ShouldFailToVerifyBase64WithPkcs1()
        {
            var plainBytes = Encoding.UTF8.GetBytes("hello, 世界");
            var pri = await RSAKeyFactory.ImportPrivateKeyFromPemFileAsync("./keys/self_pkcs1.key");
            var pub = await RSAKeyFactory.ImportPublicKeyFromPemFileAsync("./keys/self_pkcs1.pub");
            ISignatureProvider provider = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                provider.VerifyBase64Signature(null, string.Empty, HashAlgorithmName.SHA256);
            });

            provider = new RSACryptoProvider(pri, pub, RSASignaturePadding.Pkcs1);
            Assert.Throws<ArgumentNullException>(() =>
            {
                provider.VerifyBase64Signature(null, string.Empty, HashAlgorithmName.SHA256);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                provider.VerifyBase64Signature(Array.Empty<byte>(), string.Empty, HashAlgorithmName.SHA256);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                provider.VerifyBase64Signature(plainBytes, null, HashAlgorithmName.SHA256);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                provider.VerifyBase64Signature(plainBytes, string.Empty, HashAlgorithmName.SHA256);
            });

            Assert.Throws<FormatException>(() =>
            {
                provider.VerifyBase64Signature(plainBytes, "hello", HashAlgorithmName.SHA256);
            });
        }

        [Fact]
        public async void ShouldSignAndVerifyBase64WithPkcs1()
        {
            var plainBytes = Encoding.UTF8.GetBytes("hello, 世界");
            var pri = await RSAKeyFactory.ImportPrivateKeyFromPemFileAsync("./keys/self_pkcs1.key");
            var pub = await RSAKeyFactory.ImportPublicKeyFromPemFileAsync("./keys/self_pkcs1.pub");
            var provider = new RSACryptoProvider(pri, pub, RSASignaturePadding.Pkcs1);
            var signature = provider.SignToBase64String(plainBytes, HashAlgorithmName.SHA256);

            Assert.True(provider.VerifyBase64Signature(plainBytes, signature, HashAlgorithmName.SHA256));
        }

        [Fact]
        public async void ShouldSignAndVerifyBase64WithPkcs8()
        {
            var plainBytes = Encoding.UTF8.GetBytes("hello, 世界");
            var pri = await RSAKeyFactory.ImportPrivateKeyFromPemFileAsync("./keys/self_pkcs8.key");
            var pub = await RSAKeyFactory.ImportPublicKeyFromPemFileAsync("./keys/self_x509.pub");
            var provider = new RSACryptoProvider(pri, pub, RSASignaturePadding.Pkcs1);
            var signature = provider.SignToBase64String(plainBytes, HashAlgorithmName.SHA256);

            Assert.True(provider.VerifyBase64Signature(plainBytes, signature, HashAlgorithmName.SHA256));
        }

        private async Task GeneratePkcs1PublicKeyAsync()
        {
            var priKey = await RSAKeyFactory.ImportPrivateKeyFromPemFileAsync("./keys/self_pkcs1.key");
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportRSAPrivateKey(priKey.Key, out _);
            var pubKey = rsa.ExportRSAPublicKey();
            var encoded = Convert.ToBase64String(pubKey).ToCharArray();

            using var file = System.IO.File.Create("./keys/self_pkcs1.pub");
            using var writer = new StreamWriter(file);
            await writer.WriteLineAsync("-----BEGIN RSA PUBLIC KEY-----");
            for (var start = 0; start < encoded.Length; start = start + 64)
            {
                await writer.WriteLineAsync(
                  new string(encoded[new Range(start, Math.Min(encoded.Length, start + 64))]));
            }
            await writer.WriteLineAsync("-----END RSA PUBLIC KEY-----");
            await writer.FlushAsync();
            writer.Close();
        }
    }

}