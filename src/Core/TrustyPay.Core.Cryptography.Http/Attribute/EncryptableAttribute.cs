
using System;

namespace TrustyPay.Core.Cryptography.Http
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class EncryptableAttribute : Attribute
    {
        public EncryptableAttribute() { }
    }
}