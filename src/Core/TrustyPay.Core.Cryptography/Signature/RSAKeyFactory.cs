using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TrustyPay.Core.Cryptography
{
    /// <summary>
    /// RSA Key Factory
    /// </summary>
    public static class RSAKeyFactory
    {
        /// <summary>
        /// Import private key from file.
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns>RSACryptoProvider.PrivateKey</returns>
        /// <exception cref="FileNotFoundException">file path is incorrect</exception>
        public async static Task<RSACryptoProvider.PrivateKey> ImportPrivateKeyFromPemFileAsync(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"The ({path}) is invalid!");
            }

            var lines = await File.ReadAllLinesAsync(path);
            return ImportPrivateKeyLines(lines);
        }

        /// <summary>
        /// Import private key from pem text.
        /// </summary>
        /// <param name="pem">pem text</param>
        /// <returns>RSACryptoProvider.PrivateKey</returns>
        /// <exception cref="ArgumentNullException">file path is incorrect</exception>
        public static RSACryptoProvider.PrivateKey ImportPrivateKeyFromPemText(string pem)
        {
            if (string.IsNullOrEmpty(pem))
            {
                throw new ArgumentNullException(nameof(pem));
            }

            var lines = pem.Split('\n');
            return ImportPrivateKeyLines(lines);
        }
        
        /// <summary>
        /// Import private key from base64 string
        /// </summary>
        /// <param name="privateKey">base64 string</param>
        /// <param name="format">RSACryptoProvider.PrivateKeyFormat</param>
        /// <returns>RSACryptoProvider.PrivateKey</returns>
        public static RSACryptoProvider.PrivateKey ImportPrivateKeyFromBase64String(
            string privateKey, 
            RSACryptoProvider.PrivateKeyFormat format = RSACryptoProvider.PrivateKeyFormat.Pkcs1)
        {
            return new RSACryptoProvider.PrivateKey(
              Convert.FromBase64String(privateKey), format
            );
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

        /// <summary>
        /// Import public key from base64 string
        /// </summary>
        /// <param name="privateKey">base64 string</param>
        /// <param name="format">RSACryptoProvider.PublicKeyFormat</param>
        /// <returns>RSACryptoProvider.PublicKey</returns>
        public static RSACryptoProvider.PublicKey ImportPublicKeyFromBase64String(
            string publicKey, 
            RSACryptoProvider.PublicKeyFormat format = RSACryptoProvider.PublicKeyFormat.Pkcs1)
        {
            return new RSACryptoProvider.PublicKey(
              Convert.FromBase64String(publicKey), format
            );
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

        /// <summary>
        /// Import public key from cer file.
        /// </summary>
        /// <param name="path">cer file path</param>
        /// <returns>RSACryptoProvider.PublicKey</returns>
        /// <exception cref="FileNotFoundException">cer file doesn't exist!</exception>
        public static RSACryptoProvider.PublicKey ImportPublicKeyFromCerFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"The ({path}) is invalid!");
            }

            var cer = new X509Certificate2(path);
            return new RSACryptoProvider.PublicKey(
              cer.PublicKey.Key.ExportSubjectPublicKeyInfo(), RSACryptoProvider.PublicKeyFormat.X509);
        }

        /// <summary>
        /// Import private key from pfx file.
        /// </summary>
        /// <param name="path">pfx file path</param>
        /// <param name="password">pfx file password</param>
        /// <returns>RSACryptoProvider.PrivateKey</returns>
        /// <exception cref="FileNotFoundException">pfx file doesn't exist!</exception>
        public static RSACryptoProvider.PrivateKey ImportPrivateKeyFromPfxFile(string path, string password)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"The ({path}) is invalid!");
            }

            var pfx = new X509Certificate2(
              path,
              password ?? string.Empty,
              X509KeyStorageFlags.Exportable);
            return new RSACryptoProvider.PrivateKey(
              pfx.PrivateKey.ExportPkcs8PrivateKey(), RSACryptoProvider.PrivateKeyFormat.Pkcs8);
        }
    }
}