using PluginLiberty.Helper;

namespace PluginLiberty.API.Factory
{
    public interface IApiClientFactory
    {
        IApiClient CreateApiClient(Settings settings);
    }
}