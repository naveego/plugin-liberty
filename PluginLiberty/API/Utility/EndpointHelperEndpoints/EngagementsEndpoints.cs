using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Naveego.Sdk.Logging;
using Naveego.Sdk.Plugins;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PluginLiberty.API.Factory;
using PluginLiberty.DataContracts;

namespace PluginLiberty.API.Utility.EndpointHelperEndpoints
{
    public class EngagementsEndpointHelper
    {
        private class EngagementsEndpoint : Endpoint
        {
        }

        public static readonly Dictionary<string, Endpoint> EngagementsEndpoints = new Dictionary<string, Endpoint>
        {
            {
                "AllEngagements", new EngagementsEndpoint
                {
                    Id = "AllEngagements",
                    Name = "All Engagements",
                    BasePath = "/crm/v3/",
                    AllPath = "/objects/engagements",
                    PropertiesPath = "/crm/v3/properties/engagements",
                    DetailPath = "/objects/engagements/{0}",
                    DetailPropertyId = "hs_unique_creation_key",
                    SupportedActions = new List<EndpointActions>
                    {
                        EndpointActions.Get
                    },
                    PropertyKeys = new List<string>
                    {
                        "hs_unique_creation_key"
                    }
                }
            },
            {
                "UpsertEngagements", new EngagementsEndpoint
                {
                    Id = "UpsertEngagements",
                    Name = "Upsert Engagements",
                    BasePath = "/crm/v3/objects/engagements",
                    AllPath = "/",
                    PropertiesPath = "/crm/v3/properties/engagements",
                    DetailPath = "/",
                    DetailPropertyId = "hs_unique_creation_key",
                    SupportedActions = new List<EndpointActions>
                    {
                        EndpointActions.Post,
                        EndpointActions.Put
                    },
                    PropertyKeys = new List<string>
                    {
                        "hs_unique_creation_key"
                    }
                }
            },
        };
    }
}