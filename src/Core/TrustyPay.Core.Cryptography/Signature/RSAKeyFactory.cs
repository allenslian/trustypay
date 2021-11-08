using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustyPay.Core.Cryptography
{
  public static class RSAPrivateKeyFactory
  {
    public async static Task<RSACryptoProvider.PrivateKey> ImportPrivateKeyFromPemFileAsync(string path)
    {
      if (!File.Exists(path))
      {
        throw new FileNotFoundException($"The ({path}) is invalid!");
      }

      var lines = await File.ReadAllLinesAsync(path);
      return ImportPrivateKeyLines(lines);
    }

    public static RSACryptoProvider.PrivateKey ImportPrivateKeyFromPemText(string pem)
    {
      if (string.IsNullOrEmpty(pem))
      {
        throw new ArgumentNullException(nameof(pem));
      }

      var lines = pem.Split('\n');
      return ImportPrivateKeyLines(lines);
    }

    private static RSACryptoProvider.PrivateKey ImportPrivateKeyLines(string[] lines)
    {
      if (lines == null || lines.Length == 0)
      {
        throw new InvalidDataException("The text is not PEM format!");
      }

      var key = new StringBuilder(128);
      var format = RSACryptoProvider.PrivateKeyFormat.Pkcs1;
      lines.Aggregate(key, (acc, value) =>
      {
        if (value.StartsWith("-----"))
        {
          if (value.IndexOf("RSA") == -1)
          {
            format = RSACryptoProvider.PrivateKeyFormat.Pkcs8;
          }
        }
        else
        {
          acc.Append(value.TrimEnd(new char[] { '\r', '\n' }));
        }
        return acc;
      });

      return new RSACryptoProvider.PrivateKey(
        Convert.FromBase64String(key.ToString()), format
      );
    }

    public async static Task<RSACryptoProvider.PublicKey> ImportPublicKeyFromPemFileAsync(string path)
    {
      if (!File.Exists(path))
      {
        throw new FileNotFoundException($"The ({path}) is invalid!");
      }

      var lines = await File.ReadAllLinesAsync(path);
      return ImportPublicKeyLines(lines);
    }

    public static RSACryptoProvider.PrivateKey ImportPublicKeyFromPemText(string pem)
    {
      if (string.IsNullOrEmpty(pem))
      {
        throw new ArgumentNullException(nameof(pem));
      }

      var lines = pem.Split('\n');
      return ImportPublicKeyLines(lines);
    }

    private static RSACryptoProvider.PublicKey ImportPublicKeyLines(string[] lines)
    {
      if (lines == null || lines.Length == 0)
      {
        throw new InvalidDataException("The text is not PEM format!");
      }

      var key = new StringBuilder(128);
      var format = RSACryptoProvider.PublicKeyFormat.Pkcs1;
      lines.Aggregate(key, (acc, value) =>
      {
        if (value.StartsWith("-----"))
        {
          if (value.IndexOf("RSA") == -1)
          {
            format = RSACryptoProvider.PublicKeyFormat.X509;
          }
        }
        else
        {
          acc.Append(value.TrimEnd(new char[] { '\r', '\n' }));
        }
        return acc;
      });

      return new RSACryptoProvider.PublicKey(
        Convert.FromBase64String(key.ToString()), format
      );
    }
  }
}