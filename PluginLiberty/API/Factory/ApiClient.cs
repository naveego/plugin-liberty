using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Naveego.Sdk.Logging;
using Newtonsoft.Json;
using PluginLiberty.API.Utility;
using PluginLiberty.Helper;
using RestSharp;

namespace PluginLiberty.API.Factory
{
    public class ApiClient : IApiClient
    {
        private IApiAuthenticator Authenticator { get; set; }
        private static HttpClient Client { get; set; }
        private Settings Settings { get; set; }

        private const string CustomerHeaderName = "Customer";

        public ApiClient(HttpClient client, Settings settings)
        {
            Authenticator = new ApiAuthenticator(client, settings);
            Client = client;
            Settings = settings;

            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public string GetQueryDate()
        {
            return Settings.QueryStartDate;
        }

        public async Task TestConnection()
        {
            try
            {
                var uriBuilder = new UriBuilder($"{Constants.BaseApiUrl.TrimEnd('/')}/{Utility.Constants.TestConnectionPath.TrimStart('/')}");
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);

                uriBuilder.Query = query.ToString();

                var uri = new Uri(uriBuilder.ToString());


                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = uri,
                    //   Content = new StringContent("", Encoding.UTF8, "application/json")
                };
                var key = await Authenticator.GetAPINPI();
                request.Headers.Add("Customer", key);
                var token = await Authenticator.GetToken();
                request.Headers.Add("Authorization", $"Basic {token}");
                request.Headers.Add("Accept", "*/*");
                // send request
                var client = new HttpClient();
                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string path)
        {
            try
            {
                var uriBuilder = new UriBuilder($"{Constants.BaseApiUrl.TrimEnd('/')}/{path.TrimStart('/')}");
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);

                uriBuilder.Query = query.ToString();

                var uri = new Uri(uriBuilder.ToString());

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = uri,
                };

                var key = await Authenticator.GetAPINPI();
                request.Headers.Add("Customer", key);
                var token = await Authenticator.GetToken();
                request.Headers.Add("Authorization", $"Basic {token}");
                request.Headers.Add("Accept", "*/*");

                return await Client.SendAsync(request);
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> PostAsync(string path, StringContent json)
        {
            try
            {
                var token = await Authenticator.GetToken();
                var uriBuilder = new UriBuilder($"{Constants.BaseApiUrl.TrimEnd('/')}/{path.TrimStart('/')}");
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);

                uriBuilder.Query = query.ToString();

                var uri = new Uri(uriBuilder.ToString());

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = uri,
                    Content = json
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);

                var customerString = $"{Settings.NPI}:{Settings.ApiKey}";
                var base64EncodedCustomerString =
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(customerString));

                request.Headers.Add("Password", base64EncodedCustomerString);

                return await Client.SendAsync(request);
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> PutAsync(string path, StringContent json)
        {
            try
            {
                var token = await Authenticator.GetToken();
                var uriBuilder = new UriBuilder($"{Constants.BaseApiUrl.TrimEnd('/')}/{path.TrimStart('/')}");
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);

                uriBuilder.Query = query.ToString();

                var uri = new Uri(uriBuilder.ToString());

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = uri,
                    Content = json
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);

                var customerString = $"{Settings.NPI}:{Settings.ApiKey}";
                var base64EncodedCustomerString =
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(customerString));

                request.Headers.Add("ApiKey", base64EncodedCustomerString);

                return await Client.SendAsync(request);
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> PatchAsync(string path, StringContent json)
        {
            try
            {
                var token = await Authenticator.GetToken();
                var uriBuilder = new UriBuilder($"{Constants.BaseApiUrl.TrimEnd('/')}/{path.TrimStart('/')}");
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);

                uriBuilder.Query = query.ToString();

                var uri = new Uri(uriBuilder.ToString());

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Patch,
                    RequestUri = uri,
                    Content = json
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);

                var customerString = $"{Settings.NPI}:{Settings.ApiKey}";
                var base64EncodedCustomerString =
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(customerString));

                request.Headers.Add("NPI", base64EncodedCustomerString);

                return await Client.SendAsync(request);
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> DeleteAsync(string path)
        {
            try
            {
                var token = await Authenticator.GetToken();
                var uriBuilder = new UriBuilder($"{Constants.BaseApiUrl.TrimEnd('/')}/{path.TrimStart('/')}");
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);

                uriBuilder.Query = query.ToString();

                var uri = new Uri(uriBuilder.ToString());

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = uri,
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);

                var customerString = $"{Settings.NPI}:{Settings.ApiKey}";
                var base64EncodedCustomerString =
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(customerString));

                request.Headers.Add("Customer", base64EncodedCustomerString);

                return await Client.SendAsync(request);
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }
    }
}