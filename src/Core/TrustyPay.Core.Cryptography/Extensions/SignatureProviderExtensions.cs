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
            return signatureBytes.ToBase64String();
        }

        public static bool VerifyBase64Signature(
            this ISignatureProvider source,
            byte[] plainBytes, string signatureText, HashAlgorithmName hashAlgorithm)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            // if (string.IsNullOrEmpty(signatureText))
            // {
            //     throw new ArgumentNullException(nameof(signatureText));
            // }

            var signatureBytes = signatureText.FromBase64String();
            return source.Verify(plainBytes, signatureBytes, hashAlgorithm);
        }
    }
}