
namespace TrustyPay.Core.Cryptography
{
    /// <summary>
    /// An digest provider (hasher)
    /// </summary>
    public interface IDigestProvider
    {
        /// <summary>
        /// Hash will hash a plain byte array to the hash one.
        /// </summary>
        /// <param name="plainBytes">plain byte array</param>
        /// <returns>hash byte array</returns>
        byte[] Hash(byte[] plainBytes);
    }
}