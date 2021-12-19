
namespace TrustyPay.Core.Cryptography.Http
{
    /// <summary>
    /// A encryptable object
    /// </summary>
    public interface IEncryptableObject
    {
        /// <summary>
        /// Encrypt the properties of the object
        /// </summary>
        /// <param name="charset">plain text charset</param>
        void Encrypt(string charset = "utf-8");
    }
}