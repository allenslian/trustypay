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

        public static string SignToHexString(
            this ISignatureProvider source,
            byte[] plainBytes, HashAlgorithmName hashAlgorithm)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var signatureBytes = source.Sign(plainBytes, hashAlgorithm);
            return signatureBytes.ToHexString();
        }

        public static bool VerifyBase64Signature(
            this ISignatureProvider source,
            byte[] plainBytes, string signatureText, HashAlgorithmName hashAlgorithm)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var signatureBytes = signatureText.FromBase64String();
            return source.Verify(plainBytes, signatureBytes, hashAlgorithm);
        }

        public static bool VerifyHexSignature(
            this ISignatureProvider source,
            byte[] plainBytes, string signatureText, HashAlgorithmName hashAlgorithm)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var signatureBytes = signatureText.FromHexString();
            return source.Verify(plainBytes, signatureBytes, hashAlgorithm);
        }
    }
}