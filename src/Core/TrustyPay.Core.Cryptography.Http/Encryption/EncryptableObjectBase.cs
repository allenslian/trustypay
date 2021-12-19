using System.Collections;
using System.Reflection;

namespace TrustyPay.Core.Cryptography.Http
{
    public abstract class EncryptableObjectBase : IEncryptableObject, IDecryptableObject
    {
        protected abstract IEncryptionProvider Encryptor { get; set; }

        /// <inheritdoc />
        public void Encrypt(string charset = "utf-8")
        {
            if (Encryptor == null)
            {
                return;
            }

            var props = this.GetType().GetProperties();
            foreach (var p in props)
            {
                var attributes = p.GetCustomAttributes(typeof(EncryptableAttribute), false);
                if (attributes.Length == 0)
                {
                    continue;
                }

                EncryptInternal(p, this, charset);
            }
        }

        protected void EncryptInternal(PropertyInfo p, object self, string charset)
        {
            var v = p.GetValue(self);
            if (v == null)
            {
                return;
            }

            if (v is IList l)
            {
                for (var i = 0; i < l.Count; i++)
                {
                    l[i] = GetValueEncrypted(l[i], charset);
                }
            }
            else
            {
                p.SetValue(self, GetValueEncrypted(v, charset));
            }
        }

        protected virtual object GetValueEncrypted(object v, string charset)
        {
            if (v is string s)
            {
                if (s != string.Empty)
                {
                    return Encryptor.EncryptToBase64String(s.FromCharsetString(charset));
                }
                return s;
            }
            else if (v is IEncryptableObject o)
            {
                o.Encrypt(charset);
                return o;
            }

            return v;
        }

        /// <inheritdoc />
        public void Decrypt(string charset = "utf-8")
        {
            if (Encryptor == null)
            {
                return;
            }

            var props = this.GetType().GetProperties();
            foreach (var p in props)
            {
                var attributes = p.GetCustomAttributes(typeof(EncryptableAttribute), false);
                if (attributes.Length == 0)
                {
                    continue;
                }

                DecryptInternal(p, this, charset);
            }
        }

        protected void DecryptInternal(PropertyInfo p, object self, string charset)
        {
            var v = p.GetValue(self);
            if (v == null)
            {
                return;
            }

            if (v is IList l)
            {
                for (var i = 0; i < l.Count; i++)
                {
                    l[i] = GetValueDecrypted(l[i], charset);
                }
            }
            else
            {
                p.SetValue(self, GetValueDecrypted(v, charset));
            }
        }

        protected virtual object GetValueDecrypted(object v, string charset)
        {
            if (v is string s)
            {
                if (s != string.Empty)
                {
                    return Encryptor.DecryptFromBase64String(s).ToCharsetString(charset);
                }
                return s;
            }
            else if (v is IDecryptableObject o)
            {
                o.Decrypt(charset);
                return o;
            }

            return v;
        }
    }
}