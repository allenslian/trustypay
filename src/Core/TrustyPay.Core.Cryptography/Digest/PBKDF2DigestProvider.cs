using System;
using System.Security.Cryptography;

namespace TrustyPay.Core.Cryptography
{
    /// <summary>
    /// Password-Based Key Derivation Function 2
    /// </summary>
    public class PBKDF2DigestProvider : IDigestProvider
    {
        /// <summary>
        /// min salt size
        /// </summary>
        private const int MinimumSaltSize = 8;

        /// <summary>
        /// Salt
        /// </summary>
        private readonly byte[] _salt;

        /// <summary>
        /// Iteration count
        /// </summary>
        private readonly int _iterations;

        /// <summary>
        /// Hash size
        /// </summary>
        private readonly int _hashSize;

        /// <summary>
        /// Hash algorithm name
        /// </summary>
        private readonly HashAlgorithmName _hashAlgorithmName;

        /// <summary>
        /// A PBKDF2 digest provider
        /// </summary>
        /// <param name="salt">Salt value</param>
        /// <param name="hashAlgorithmName">Hash algorithm</param>
        /// <param name="iterations">Iteration count</param>
        /// <param name="hashSize">Hash size</param>
        /// <exception cref="ArgumentNullException">Throws the exception when salt is null</exception>
        /// <exception cref="ArgumentException">
        /// Throws the exception when salt's length < 8, or iterations < 1, or hashSize < 1 
        /// </exception>
        public PBKDF2DigestProvider(byte[] salt, HashAlgorithmName hashAlgorithmName, 
            int iterations = 10000, int hashSize = 16)
        {
            if (salt == null || salt.Length == 0)
            {
                throw new ArgumentNullException(nameof(salt));
            }

            if (salt.Length < MinimumSaltSize)
            {
                throw new ArgumentException("salt size should be greater than 8!!!", nameof(salt));
            }

            if (hashAlgorithmName == HashAlgorithmName.MD5)
            {
                throw new ArgumentException("does not support MD5 algorithm!", nameof(hashAlgorithmName));
            }
            
            if (iterations < 1)
            {
                throw new ArgumentException("iterations should be greater than 0!!!", nameof(iterations));
            }

            if (hashSize < 1)
            {
                throw new ArgumentException("keySize should be greater than 0!!!", nameof(hashSize));
            }

            _salt = salt;
            _hashAlgorithmName = hashAlgorithmName;
            _iterations = iterations;
            _hashSize = hashSize;
        }

        public byte[] Hash(byte[] plainBytes)
        {
            if (plainBytes == null || plainBytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(plainBytes));
            }
            using var pbkdf2 = new Rfc2898DeriveBytes(plainBytes, 
                _salt, _iterations, _hashAlgorithmName);
            return pbkdf2.GetBytes(_hashSize);
        }
    }
}