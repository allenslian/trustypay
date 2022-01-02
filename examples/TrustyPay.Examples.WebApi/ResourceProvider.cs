

using System.Threading.Tasks;
using TrustyPay.Core.Cryptography;
using TrustyPay.Core.Cryptography.Http.Service;

namespace TrustyPay.Examples.WebApi
{
    internal class ResourceProvider : IResourceProvider
    {
        public Task<RSACryptoProvider.PublicKey> GetAppPublicKeyAsync(string appId, string apiKey)
        {
            return Task.FromResult(RSAKeyFactory.ImportPublicKeyFromBase64String(
                "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxixN2jqBN9uvf5WYvcfJA1zxY73OdPwummgQiYiR08+Ep0UY3KAhrA6ZO+Z87LBgtbFlNO+YJAHxaUo+ECskTt9bBxZYx5iKzN4QgC2vqitS0R0JTbn84vMvPQ8Onnt11UqBOEnDUuQF+t/K9/Rp77LlrGLVvZpmj3JBZqmgS8qU1TmlZEHEF7bKf9+KBimrOennVMWBdTAmsy8gx2vkdp3B0liI23XKqvgee+d4OnWF4n/6jRsU+cCI4JRDGQp47Hfy+WIwjJB7DQ7YNux0jyhdUlND0EDmXQ42O86DItnKS+svdQR9EWQN5hvn+/ftBiNcK+3wQ9whXq/OLXt3VwIDAQAB",
                RSACryptoProvider.PublicKeyFormat.X509
            ));
        }

        public RSACryptoProvider.PrivateKey GetPlatformPrivateKey()
        {
            return RSAKeyFactory.ImportPrivateKeyFromBase64String(
                "MIIEowIBAAKCAQEAxixN2jqBN9uvf5WYvcfJA1zxY73OdPwummgQiYiR08+Ep0UY3KAhrA6ZO+Z87LBgtbFlNO+YJAHxaUo+ECskTt9bBxZYx5iKzN4QgC2vqitS0R0JTbn84vMvPQ8Onnt11UqBOEnDUuQF+t/K9/Rp77LlrGLVvZpmj3JBZqmgS8qU1TmlZEHEF7bKf9+KBimrOennVMWBdTAmsy8gx2vkdp3B0liI23XKqvgee+d4OnWF4n/6jRsU+cCI4JRDGQp47Hfy+WIwjJB7DQ7YNux0jyhdUlND0EDmXQ42O86DItnKS+svdQR9EWQN5hvn+/ftBiNcK+3wQ9whXq/OLXt3VwIDAQABAoIBADURfC+qZxwcOl0CJIr9yziZVRMOqxDsz1YN9A/AgLyl37IjcMr0HtBCgIpn6KBBg0RkouOQHb/WvV0iwof15Z0xduDo/RFGKjU+alDI6ze4rk7NZcZove3QjZ/ePl32VdGuR/hY2HOEGI3cDDBmRVApKhQFy5Mgm8JiKF1jo0doGcYwzDDxjKj2KXmYKiXyyMS3serWxu2fTgaIzNf+X09rfx0+V7YSeixhSyrkZuPBZ0zwcKtbiEScP77w84ydqPCbBPC2B8GM1ydC0m7pm9dHzpzCoiMZEFxwlfW4QnUEs6ZBeOkdBi8u5qvNO9ebV69VKZtuPRJULeGOcFRihtECgYEA6gZiY5a/vZHoMZkH6980nCt2pNhjOeTXcbUz472h0vulVufTVvwbfOKUzuVgfm2FOE+duJwQ9CZ1XmxJn8JMCXcZiBbsJ8sg9Bg/hrrxlmLDCccg/hr3YXRow1GROx3bjLwXphyVdLpP8kq3J438kDk5uAIivk+Ag/fYnYWykPMCgYEA2MgX5o0JoYt0ais7GqoM+Iqi5CjC/npJqJEBWEZlDLLp+ATXzRIrta/ytOfmM8Q9mSD+Pa9lRclWeuXxRYWyMFoImEhwQEASA9GReW1Vh21BGBRLhhHHnb4vQpHPd/ZrIvD8408R5BL4mV1uCL0602lFlVWV+Y2l2+zNTUNMOQ0CgYEAumoTfZjbazoKZ1erA6xsz+mfPHhRshAjvaPFjafEe7eQYMWRhzyS+MvUFWqJjqvW7qAc1q0apCDuZSSFEQlIYFHKuKjpvFkGCuo10DChuFU7X1KLaV45qBt+R/d1ZE3IUTaS7/Lc6npurGXvt0ZW5Ntwqq6o1kel63lFx1R0hRkCgYAIVDfG3ehe0pLYeWy964awDfOQPJixWlV2KowYriu3vzAKHXWFJYuUYXw7wyUvG/0Z4xChohmMAt5Vvnv0pdxgyzTFVRMrBMssZmLmfXLpzyLPIAh+0DQRNXtvXVbRTyByqTuuKB2R28C9c7+EinC2KrdHs5AdlHmo54JD07AcCQKBgHnPy6gf65dkUFRyWMZAnfLFIoZWqzskvK2ecqfIbmNwgSnOSdj5nQBxESWoCJyPhLsUz9uo4nrC2eceF2IOYe2RmGIA79Qqv/nT3FOumGq/IY6KN2kySdiHi3WBrCRO4mSRuWLz3lDYbmqtUI+441/R3b8DUUkA5KfebsF7i/A2",
                RSACryptoProvider.PrivateKeyFormat.Pkcs1
            );
        }

        public Task<bool> HasPermissionByApiKeyAsync(string apiKey, string path, string method)
        {
            return Task.FromResult(true);
        }
    }
}