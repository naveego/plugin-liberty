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
    public class PrescriptionEndpointHelper
    {
        private class PrescriptionEndpoint : Endpoint
        {
            public override async Task<Schema> GetStaticSchemaAsync(IApiClient apiClient, Schema schema)
            {
                var response = await apiClient.GetAsync(AllPath);

                if (!response.IsSuccessStatusCode)
                {
                    var error = JsonConvert.DeserializeObject<ApiError>(await response.Content.ReadAsStringAsync());
                    throw new Exception(error.Message);
                }

                var objectPropertiesResponse =
                    JsonConvert.DeserializeObject<PropertyResponseWrapper>(
                        await response.Content.ReadAsStringAsync());

                var properties = new List<Property>();
                var row = objectPropertiesResponse.Scripts[0];

                foreach (var key in row)
                {
                    switch (key.Key)
                    {
                        case ("Patient"):
                            properties.Add(new Property
                            {
                                Id = "PatientId",
                                Name = "PatientId",
                                Description = "",
                                Type = PropertyType.String,
                                TypeAtSource = "String",
                                IsKey = false,
                                IsNullable = true
                            });
                            break;
                        case ("ScriptNumber"):
                            properties.Add(new Property
                            {
                                Id = "ScriptNumber",
                                Name = "ScriptNumber",
                                Description = "",
                                Type = PropertyType.String,
                                TypeAtSource = "String",
                                IsKey = true,
                                IsNullable = false
                            });
                            break;
                        default:
                            properties.Add(new Property
                            {
                                Id = key.Key,
                                Name = key.Key,
                                Description = "",
                                Type = PropertyType.String,
                                TypeAtSource = "String",
                                IsKey = false,
                                IsNullable = true
                            });
                            break;
                    }
                }

                schema.Properties.Clear();
                schema.Properties.AddRange(properties);

                if (schema.Properties.Count == 0)
                {
                    schema.Description = Constants.EmptySchemaDescription;
                }

                schema.DataFlowDirection = GetDataFlowDirection();

                return schema;
            }
            public override async IAsyncEnumerable<Record> ReadRecordsAsync(IApiClient apiClient, Schema schema, bool isDiscoverRead = false)
            {
                var queryDate = apiClient.GetQueryDate();
                var pageNumber = 1;

                while (true)
                {
                    var response = await apiClient.GetAsync(
                        $"{AllPath.TrimStart('/')}&StartDate={queryDate}&Page={pageNumber}");


                    response.EnsureSuccessStatusCode();

                    var objectResponseWrapper =
                        JsonConvert.DeserializeObject<PrescriptionResponseWrapper>(await response.Content.ReadAsStringAsync());

                    if (objectResponseWrapper?.Scripts.Count == 0)
                    {
                        yield break;
                    }
                    else
                    {
                        foreach (var objectResponse in objectResponseWrapper?.Scripts)
                        {
                            var recordMap = new Dictionary<string, object>();

                            foreach (var objectProperty in objectResponse)
                            {
                                switch (objectProperty.Key)

                                {
                                    case ("Patient"):
                                        try
                                        {
                                            var patientJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(objectProperty.Value.ToString());
                                            recordMap["PatientId"] = patientJson["Id"] ?? "";
                                        }
                                        catch
                                        {
                                            recordMap[objectProperty.Key] = "";
                                        }
                                        break;

                                    default:

                                        try
                                        {
                                            recordMap[objectProperty.Key] = objectProperty.Value.ToString() ?? "";
                                        }
                                        catch
                                        {
                                            recordMap[objectProperty.Key] = "";
                                        }
                                        break;
                                }
                            }

                            yield return new Record
                            {
                                Action = Record.Types.Action.Upsert,
                                DataJson = JsonConvert.SerializeObject(recordMap)
                            };
                        }
                        pageNumber = pageNumber + 1;
                    }
                }
            }

            public async Task<Schema> GetSchemaForEndpoint(IApiClient apiClient, Schema schema, Endpoint? endpoint)
            {
                if (endpoint == null)
                {
                    return schema;
                }

                if (endpoint.ShouldGetStaticSchema)
                {
                    return await endpoint.GetStaticSchemaAsync(apiClient, schema);
                }

                // invoke properties api
                var response = await apiClient.GetAsync(endpoint.AllPath);

                if (!response.IsSuccessStatusCode)
                {
                    var error = JsonConvert.DeserializeObject<ApiError>(await response.Content.ReadAsStringAsync());
                    throw new Exception(error.Message);
                }

                var objectPropertiesResponse =
                    JsonConvert.DeserializeObject<PropertyResponseWrapper>(
                        await response.Content.ReadAsStringAsync());

                var properties = new List<Property>();
                var row = objectPropertiesResponse.Scripts[0];

                foreach (var key in row)
                {
                    switch (key.Key)
                    {
                        case ("Patient"):
                            properties.Add(new Property
                            {
                                Id = "PatientId",
                                Name = "PatientId",
                                Description = "",
                                Type = PropertyType.String,
                                TypeAtSource = "String",
                                IsKey = false,
                                IsNullable = true
                            });
                            break;
                        default:
                            properties.Add(new Property
                            {
                                Id = key.Key,
                                Name = key.Key,
                                Description = "",
                                Type = PropertyType.String,
                                TypeAtSource = "String",
                                IsKey = false,
                                IsNullable = true
                            });
                            break;
                    }
                }

                schema.Properties.Clear();
                schema.Properties.AddRange(properties);

                if (schema.Properties.Count == 0)
                {
                    schema.Description = Constants.EmptySchemaDescription;
                }

                schema.DataFlowDirection = endpoint.GetDataFlowDirection();

                return schema;
            }
        }
        public static readonly Dictionary<string, Endpoint> PrescriptionEndpoints = new Dictionary<string, Endpoint>
        {
            {
                "AllPrescriptions", new PrescriptionEndpoint
                {
                    Id = "AllPrescriptions",
                    ShouldGetStaticSchema = true,
                    Name = "AllPrescriptions",
                    BasePath = "https://api.libertysoftware.com",
                    AllPath = $"/prescriptions?PageSize=100",
                    PropertiesPath = "/crm/v3/properties/prescriptions",
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
        };
    }
}