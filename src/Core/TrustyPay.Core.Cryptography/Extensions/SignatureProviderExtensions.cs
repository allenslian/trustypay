using System;
using System.Security.Cryptography;

namespace TrustyPay.Core.Cryptography
{
    /// <summary>
    /// An extension for ISignatureProvider
    /// </summary>
    public static class SignatureProviderExtensions
    {
        public static string SignToBase64String(
            this ISignatureProvider source,
            byte[] plainBytes, HashAlgorithmName hashAlgorithm)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var signatureBytes = source.Sign(plainBytes, hashAlgorithm);
            return Convert.ToBase64String(signatureBytes);
        }

        public static bool VerifyBase64Signature(
            this ISignatureProvider source,
            byte[] plainBytes, string signatureText, HashAlgorithmName hashAlgorithm)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var signatureBytes = Convert.FromBase64String(signatureText);
            return source.Verify(plainBytes, signatureBytes, hashAlgorithm);
        }
    }
}