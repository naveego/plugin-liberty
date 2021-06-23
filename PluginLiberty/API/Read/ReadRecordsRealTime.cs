using System;
using System.Threading.Tasks;
using Grpc.Core;
using Naveego.Sdk.Logging;
using Naveego.Sdk.Plugins;
using Newtonsoft.Json;
using PluginLiberty.API.Factory;

namespace PluginLiberty.API.Read
{
    public static partial class Read
    {
        public static async Task<long> ReadRecordsRealTimeAsync(IApiClient apiClient, ReadRequest request,
            IServerStreamWriter<Record> responseStream,
            ServerCallContext context)
        {
            throw new NotImplementedException();
        }
    }
}