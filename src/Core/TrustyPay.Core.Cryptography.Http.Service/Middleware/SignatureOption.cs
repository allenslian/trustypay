
namespace TrustyPay.Core.Cryptography.Http.Service
{
    /// <summary>
    /// SignatureMiddleware option
    /// </summary>
    public class SignatureOption
    {
        public SignatureOption()
        {
            AppIdName = "TrustyPay:MW:AppId";
            ValidateTimestamp = true;
        }

        /// <summary>
        /// App Id name for HttpContext.Items
        /// </summary>
        public string AppIdName { get; internal set; }

        public bool ValidateTimestamp { get; internal set; }
    }
}