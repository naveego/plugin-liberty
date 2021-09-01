using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Naveego.Sdk.Plugins;
using Newtonsoft.Json;
using PluginLiberty.API.Factory;
using PluginLiberty.API.Utility;
using PluginLiberty.DataContracts;
using PluginLiberty.Helper;

namespace PluginLiberty.API.Discover
{
    public static partial class Discover
    {
        public static async IAsyncEnumerable<Schema> GetAllSchemas(IApiClient apiClient, Settings settings,
            int sampleSize = 5)
        {
            var allEndpoints = EndpointHelper.GetAllEndpoints();

            foreach (var endpoint in allEndpoints.Values)
            {
                // base schema to be added to
                var schema = new Schema
                {
                    Id = endpoint.Id,
                    Name = endpoint.Name,
                    Description = "",
                    PublisherMetaJson = JsonConvert.SerializeObject(endpoint),
                    DataFlowDirection = endpoint.GetDataFlowDirection()
                };

                schema = await GetSchemaForEndpoint(apiClient, schema, endpoint);

                // get sample and count
                yield return await AddSampleAndCount(apiClient, schema, sampleSize, endpoint);
            }
        }

        private static async Task<Schema> AddSampleAndCount(IApiClient apiClient, Schema schema,
            int sampleSize, Endpoint? endpoint)
        {
            if (endpoint == null)
            {
                return schema;
            }

            // add sample and count
            var records = Read.Read.ReadRecordsAsync(apiClient, schema).Take(sampleSize);
            schema.Sample.AddRange(await records.ToListAsync());
            schema.Count = await GetCountOfRecords(apiClient, endpoint);

            return schema;
        }

        private static async Task<Schema> GetSchemaForEndpoint(IApiClient apiClient, Schema schema, Endpoint? endpoint)
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
            // var propertyMetaJson = new PropertyMetaJson
            // Calculated = objectProperty.Calculated,
            // IsKey = objectProperty.IsKey,
            // ModificationMetaData = objectProperty.ModificationMetaData
             //   };

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
}