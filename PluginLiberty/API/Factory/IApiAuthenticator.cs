using System.Threading.Tasks;

namespace PluginLiberty.API.Factory
{
    public interface IApiAuthenticator
    {
        Task<string> GetToken();
    }
}