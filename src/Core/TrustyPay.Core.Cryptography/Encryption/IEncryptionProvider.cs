using System;
using System.Threading.Tasks;

namespace TrustyPay.Core.Cryptography
{
    /// <summary>
    /// An encryption/decryption provider
    /// </summary>
    public interface IEncryptionProvider
    {
        /// <summary>
        /// Encrypt will encrypt a plain text to the cipher one.
        /// </summary>
        /// <param name="plainBytes">plain byte array</param>
        /// <returns>cipher byte array</returns>
        byte[] Encrypt(byte[] plainBytes);

        /// <summary>
        /// Decrypt will decrypt a cipher text to the plain one.
        /// </summary>
        /// <param name="cipherBytes">cipher byte array</param>
        /// <returns>plain byte array</returns>
        byte[] Decrypt(byte[] cipherBytes);
    }
}
