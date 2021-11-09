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
    public async void ShouldSignAndVerifyWithPkcs8()
    {
      var pri = await RSAPrivateKeyFactory.ImportPrivateKeyFromPemFileAsync("./keys/self_pkcs8.key");
      var pub = await RSAPrivateKeyFactory.ImportPublicKeyFromPemFileAsync("./keys/self_x509.pub");
      var plainBytes = Encoding.ASCII.GetBytes("hello world!");
      var provider = new RSACryptoProvider(pri, pub, RSASignaturePadding.Pkcs1);
      var signature = provider.Sign(plainBytes, HashAlgorithmName.SHA256);
      Assert.True(provider.Verify(plainBytes, signature, HashAlgorithmName.SHA256));
    }

    [Fact]
    public async void ShouldSignAndVerifyWithPkcs1()
    {
      // await GeneratePkcs1PublicKeyAsync();
      var pri = await RSAPrivateKeyFactory.ImportPrivateKeyFromPemFileAsync("./keys/self_pkcs1.key");
      var pub = await RSAPrivateKeyFactory.ImportPublicKeyFromPemFileAsync("./keys/self_pkcs1.pub");
      var plainBytes = Encoding.ASCII.GetBytes("hello world!");
      var provider = new RSACryptoProvider(pri, pub, RSASignaturePadding.Pkcs1);
      var signature = provider.Sign(plainBytes, HashAlgorithmName.SHA256);
      Assert.True(provider.Verify(plainBytes, signature, HashAlgorithmName.SHA256));
    }

    [Fact]
    public async void ShouldEncryptAndDecryptWithPkcs8()
    {
      var pri = await RSAPrivateKeyFactory.ImportPrivateKeyFromPemFileAsync("./keys/self_pkcs8.key");
      var pub = await RSAPrivateKeyFactory.ImportPublicKeyFromPemFileAsync("./keys/self_x509.pub");
      var plainBytes = Encoding.ASCII.GetBytes("hello world!");
      var provider = new RSACryptoProvider(pri, pub, RSAEncryptionPadding.Pkcs1);
      var cipherBytes = provider.Encrypt(plainBytes);
      plainBytes = provider.Decrypt(cipherBytes);
      Assert.Equal("hello world!", Encoding.ASCII.GetString(plainBytes));
    }

    [Fact]
    public async void ShouldEncryptAndDecryptWithPkcs1()
    {
      var pri = await RSAPrivateKeyFactory.ImportPrivateKeyFromPemFileAsync("./keys/self_pkcs1.key");
      var pub = await RSAPrivateKeyFactory.ImportPublicKeyFromPemFileAsync("./keys/self_pkcs1.pub");
      var plainBytes = Encoding.ASCII.GetBytes("hello world!");
      var provider = new RSACryptoProvider(pri, pub, RSAEncryptionPadding.Pkcs1);
      var cipherBytes = provider.Encrypt(plainBytes);
      plainBytes = provider.Decrypt(cipherBytes);
      Assert.Equal("hello world!", Encoding.ASCII.GetString(plainBytes));
    }

    private async Task GeneratePkcs1PublicKeyAsync()
    {
      var priKey = await RSAPrivateKeyFactory.ImportPrivateKeyFromPemFileAsync("./keys/self_pkcs1.key");
      var rsa = new RSACryptoServiceProvider();
      rsa.ImportRSAPrivateKey(priKey.Key, out _);
      var pubKey = rsa.ExportRSAPublicKey();
      var encoded = Convert.ToBase64String(pubKey).ToCharArray();

      using var file = System.IO.File.Create("./keys/self_pkcs1.pub");
      using var writer = new StreamWriter(file);
      await writer.WriteLineAsync("-----BEGIN RSA PUBLIC KEY-----");
      for (var start = 0; start < encoded.Length; start=start+64)
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