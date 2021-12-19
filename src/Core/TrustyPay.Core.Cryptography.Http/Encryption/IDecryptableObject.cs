
namespace TrustyPay.Core.Cryptography.Http
{
    /// <summary>
    /// A decryptable object
    /// </summary>
    public interface IDecryptableObject
    {
        /// <summary>
        /// Decrypt the properties of the object
        /// </summary>
        /// <param name="charset">plain text charset</param>
        void Decrypt(string charset = "utf-8");
    }
}