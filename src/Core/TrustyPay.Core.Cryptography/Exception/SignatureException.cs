using System;

namespace TrustyPay.Core.Cryptography
{
    public class SignatureException : CryptoExceptionBase
    {
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="message">error messaage</param>
        public SignatureException(string message) : base(message) { }

        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="message">error messaage</param>
        /// <param name="innerException">inner exception</param>
        public SignatureException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="message">error messaage</param>
        /// <param name="paramName">parameter name</param>
        public SignatureException(string message, string paramName) : base($"{paramName}: {message}") { }
    }
}