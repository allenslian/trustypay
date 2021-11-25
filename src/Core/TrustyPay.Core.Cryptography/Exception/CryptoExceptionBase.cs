
using System;

namespace TrustyPay.Core.Cryptography
{
    public abstract class CryptoExceptionBase : Exception
    {
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="message">error messaage</param>
        public CryptoExceptionBase(string message) : base(message) { }

        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="message">error messaage</param>
        /// <param name="innerException">inner exception</param>
        public CryptoExceptionBase(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="message">error messaage</param>
        /// <param name="paramName">parameter name</param>
        public CryptoExceptionBase(string message, string paramName) : base($"{paramName}: {message}") { }
    }
}