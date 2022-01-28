using System;
using System.Collections;
using System.Reflection;
using System.Text;

namespace TrustyPay.Core.Cryptography.Http
{
    public sealed class ObjectEncryptor
    {
        private Encoding _encoding;

        private IEncryptionProvider Encryptor { get; set; }

        public ObjectEncryptor(IEncryptionProvider provider) : this(provider, Encoding.UTF8)
        {
        }

        public ObjectEncryptor(IEncryptionProvider provider, Encoding encoding)
        {
            Encryptor = provider ?? throw new ArgumentNullException(nameof(provider));
            _encoding = encoding;
        }

        public void Encrypt(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            int level = 1;
            EncryptInternal(true, value, value.GetType().GetProperties(), ref level);
        }

        public void Decrypt(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            int level = 1;
            EncryptInternal(false, value, value.GetType().GetProperties(), ref level);
        }

        private void EncryptInternal(bool encrypted, object value, PropertyInfo[] props, ref int level)
        {
            if (value == null || props == null || props.Length == 0)
            {
                return;
            }

            foreach (var p in props)
            {
                var pv = p.GetValue(value);
                if (pv == null)
                {
                    continue;
                }

                var attr = p.GetCustomAttribute<EncryptableAttribute>(false);
                if (attr == null)
                {
                    continue;
                }

                if (pv is string s1)
                {
                    if (s1 != string.Empty && p.CanWrite)
                    {
                        p.SetValue(value, encrypted
                            ? Encryptor.EncryptToBase64String(_encoding.GetBytes(s1))
                            : _encoding.GetString(Encryptor.DecryptFromBase64String(s1)));
                    }
                }
                else if (pv is IList list)
                {
                    for (var i = 0; i < list.Count; i++)
                    {
                        if (list[i] == null)
                        {
                            continue;
                        }

                        if (list[i] is string s2)
                        {
                            if (s2 != string.Empty)
                            {
                                list[i] = encrypted
                                    ? Encryptor.EncryptToBase64String(_encoding.GetBytes(s2))
                                    : _encoding.GetString(Encryptor.DecryptFromBase64String(s2));
                            }
                        }
                        else
                        {
                            EncryptInternal(encrypted, list[i], list[i].GetType().GetProperties(), ref level);
                        }
                    }
                }
                else
                {
                    EncryptInternal(encrypted, pv, pv.GetType().GetProperties(), ref level);
                }
            }
        }
    }
}