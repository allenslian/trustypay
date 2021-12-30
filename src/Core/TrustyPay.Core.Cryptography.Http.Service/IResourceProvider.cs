
using System.Threading.Tasks;

namespace TrustyPay.Core.Cryptography.Http.Service
{
    public interface IResourceProvider
    {
        Task<bool> HasPermissionByApiKeyAsync(string apiKey, string path, string method);

        Task<RSACryptoProvider.PublicKey> GetAppPublicKeyAsync(string appId, string apiKey);

        RSACryptoProvider.PrivateKey GetPlatformPrivateKey();
    }
}