using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Naveego.Sdk.Logging;
using Newtonsoft.Json;
using PluginLiberty.DataContracts;
using PluginLiberty.Helper;

namespace PluginLiberty.API.Factory
{
    public class ApiAuthenticator : IApiAuthenticator
    {
        private HttpClient Client { get; set; }
        private Settings Settings { get; set; }
        private string Token { get; set; }
        private DateTime ExpiresAt { get; set; }

        public ApiAuthenticator(HttpClient client, Settings settings)
        {
            Client = client;
            Settings = settings;
            ExpiresAt = DateTime.Now;
            Token = "";
        }

        public async Task<string> GetToken()
        {
            return await GetNewToken();
        }

        private async Task<string> GetNewToken()
        {
            var authenticationString = $"{Settings.Username}:{Settings.Password}";
            var base64EncodedAuthenticationString =
                Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationString));

            return base64EncodedAuthenticationString;
        }
        public async Task<string> GetAPINPI()
        {
            return await GetAPINPIToken();
        }

        private async Task<string> GetAPINPIToken()
        {
            var authenticationString = $"{Settings.NPI}:{Settings.ApiKey}";
            var base64EncodedAuthenticationString =
                Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationString));

            return base64EncodedAuthenticationString;
        }
    }
}