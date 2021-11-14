using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustyPay.Core.Cryptography
{
  public static class RSAKeyFactory
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
      if (lines == null || lines.Length == 0 || !lines[0].StartsWith("-----"))
      {
        throw new InvalidDataException("The text is not PEM format!");
      }

      var format = RSACryptoProvider.PrivateKeyFormat.Pkcs1;
      var key = lines.Aggregate(new StringBuilder(128), (acc, value) =>
      {
        if (value.StartsWith("-----"))
        {
          if (!value.Contains("RSA", StringComparison.CurrentCultureIgnoreCase))
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

    /// <summary>
    /// Import public key by PEM file path
    /// </summary>
    /// <param name="path">PEM file path</param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException">PEM file path is invalid!</exception>
    public async static Task<RSACryptoProvider.PublicKey> ImportPublicKeyFromPemFileAsync(string path)
    {
      if (!File.Exists(path))
      {
        throw new FileNotFoundException($"The ({path}) is invalid!");
      }

      var lines = await File.ReadAllLinesAsync(path);
      return ImportPublicKeyLines(lines);
    }

    /// <summary>
    /// Import public key by PEM text
    /// </summary>
    /// <param name="pem">PEM text</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">The text is null or empty string.</exception>
    /// <exception cref="InvalidDataException">The text is not invalid PEM format!</exception>
    public static RSACryptoProvider.PublicKey ImportPublicKeyFromPemText(string pem)
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
      if (lines == null || lines.Length == 0 || !lines[0].StartsWith("-----"))
      {
        throw new InvalidDataException("The text is not PEM format!");
      }

      var format = RSACryptoProvider.PublicKeyFormat.Pkcs1;
      var key = lines.Aggregate(new StringBuilder(128), (acc, value) =>
      {
        if (value.StartsWith("-----"))
        {
          if (!value.Contains("RSA", StringComparison.CurrentCultureIgnoreCase))
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