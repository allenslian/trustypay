using System.Security.Cryptography;

namespace TrustyPay.Core.Cryptography
{
    /// <summary>
    /// A signature provider
    /// </summary>
    public interface ISignatureProvider
    {
        /// <summary>
        /// Sign will sign 
        /// </summary>
        /// <param name="plainBytes">plain byte array for signature</param>
        /// <param name="hashAlgorithm">hash algorithm</param>
        /// <returns>signature</returns>
        byte[] Sign(byte[] plainBytes, HashAlgorithmName hashAlgorithm);

        /// <summary>
        /// Verify will decrypt a cipher text to the plain one.
        /// </summary>
        /// <param name="plainBytes">plain byte array for signature</param>
        /// <param name="signatureBytes">signature</param>
        /// <param name="hashAlgorithm">hash algorithm</param>
        /// <returns>true means the signature is valid, false means the signature is invalid.</returns>
        bool Verify(byte[] plainBytes, byte[] signatureBytes, HashAlgorithmName hashAlgorithm);
    }
}