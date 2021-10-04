using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Naveego.Sdk.Plugins;
using Newtonsoft.Json;
using PluginLiberty.API.Read;
using PluginLiberty.API.Utility;
using PluginLiberty.DataContracts;
using PluginLiberty.Helper;
using Xunit;
using Record = Naveego.Sdk.Plugins.Record;

namespace PluginLibertyTest.Plugin
{
    public class PluginIntegrationTest
    {
        private Settings GetSettings()
        {
            return
                new Settings
                {
                    ApiKey = "", // add to test
                    Username = "",
                    NPI = "",
                    Password = "",
                    QueryStartDate = "2019-01-01"
                };
        }

        private ConnectRequest GetConnectSettings()
        {
            var settings = GetSettings();

            return new ConnectRequest
            {
                SettingsJson = JsonConvert.SerializeObject(settings),
                OauthConfiguration = null,
                OauthStateJson = ""
            };
        }

        private Schema GetTestSchema(string endpointId = null, string id = "AllClaims", string name = "AllClaims")
        {
            Endpoint endpoint = endpointId == null
                ? EndpointHelper.GetEndpointForId("AllClaims")
                : EndpointHelper.GetEndpointForId(endpointId);


            return new Schema
            {
                Id = id,
                Name = name,
                PublisherMetaJson = JsonConvert.SerializeObject(endpoint),
            };
        }

        [Fact]
        public async Task ConnectSessionTest()
        {
            // setup
            Server server = new Server
            {
                Services = { Publisher.BindService(new PluginLiberty.Plugin.Plugin()) },
                Ports = { new ServerPort("localhost", 0, ServerCredentials.Insecure) }
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var request = GetConnectSettings();
            var disconnectRequest = new DisconnectRequest();

            // act
            var response = client.ConnectSession(request);
            var responseStream = response.ResponseStream;
            var records = new List<ConnectResponse>();

            while (await responseStream.MoveNext())
            {
                records.Add(responseStream.Current);
                client.Disconnect(disconnectRequest);
            }

            // assert
            Assert.Single(records);

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }

        [Fact]
        public async Task ConnectTest()
        {
            // setup
            Server server = new Server
            {
                Services = { Publisher.BindService(new PluginLiberty.Plugin.Plugin()) },
                Ports = { new ServerPort("localhost", 0, ServerCredentials.Insecure) }
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var request = GetConnectSettings();

            // act
            var response = client.Connect(request);

            // assert
            Assert.IsType<ConnectResponse>(response);
            Assert.Equal("", response.SettingsError);
            Assert.Equal("", response.ConnectionError);
            Assert.Equal("", response.OauthError);

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }

        [Fact]
        public async Task DiscoverSchemasAllTest()
        {
            // setup
            Server server = new Server
            {
                Services = { Publisher.BindService(new PluginLiberty.Plugin.Plugin()) },
                Ports = { new ServerPort("localhost", 0, ServerCredentials.Insecure) }
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var connectRequest = GetConnectSettings();

            var request = new DiscoverSchemasRequest
            {
                Mode = DiscoverSchemasRequest.Types.Mode.All,
                SampleSize = 10
            };

            // act
            client.Connect(connectRequest);
            var response = client.DiscoverSchemas(request);

            // assert
            Assert.IsType<DiscoverSchemasResponse>(response);
            Assert.Equal(2, response.Schemas.Count);

            // var schema = response.Schemas[0];
            // Assert.Equal($"AllAccountsReceivable", schema.Id);
            // Assert.Equal("AllAccountsReceivable", schema.Name);
            // Assert.Equal($"", schema.Query);
            // Assert.Equal(5, schema.Sample.Count);
            // Assert.Equal(15, schema.Properties.Count);

            // var property = schema.Properties[0];
            // Assert.Equal("OwnerPatientId", property.Id);
            // Assert.Equal("OwnerPatientId", property.Name);
            // Assert.Equal("", property.Description);
            // Assert.Equal(PropertyType.String, property.Type);
            // Assert.True(property.IsKey);
            // Assert.False(property.IsNullable);

            // var schema2 = response.Schemas[1];
            // Assert.Equal($"AllPrescriptions", schema2.Id);
            // Assert.Equal("AllPrescriptions", schema2.Name);
            // Assert.Equal($"", schema2.Query);
            // Assert.Equal(10, schema2.Sample.Count);
            // Assert.Equal(104, schema2.Properties.Count);

            // var property2 = schema2.Properties[0];
            // Assert.Equal("ScriptNumber", property2.Id);
            // Assert.Equal("ScriptNumber", property2.Name);
            // Assert.Equal("", property2.Description);
            // Assert.Equal(PropertyType.String, property2.Type);
            // Assert.True(property2.IsKey);
            // Assert.False(property2.IsNullable);

            // var schema3 = response.Schemas[1];
            // Assert.Equal($"AllClaims", schema3.Id);
            // Assert.Equal("AllClaims", schema3.Name);
            // Assert.Equal($"", schema3.Query);
            // Assert.Equal(10, schema3.Sample.Count);
            // Assert.Equal(30, schema3.Properties.Count);

            // var property3 = schema3.Properties[0];
            // Assert.Equal("ScriptNumber", property3.Id);
            // Assert.Equal("ScriptNumber", property3.Name);
            // Assert.Equal("", property3.Description);
            // Assert.Equal(PropertyType.String, property3.Type);
            // Assert.True(property3.IsKey);
            // Assert.False(property3.IsNullable);

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }

        [Fact]
        public async Task DiscoverSchemasRefreshTest()
        {
            // setup
            Server server = new Server
            {
                Services = { Publisher.BindService(new PluginLiberty.Plugin.Plugin()) },
                Ports = { new ServerPort("localhost", 0, ServerCredentials.Insecure) }
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var connectRequest = GetConnectSettings();

            var request = new DiscoverSchemasRequest
            {
                Mode = DiscoverSchemasRequest.Types.Mode.Refresh,
                SampleSize = 10,
                ToRefresh =
                {
                    GetTestSchema()
                }
            };

            // act
            client.Connect(connectRequest);
            var response = client.DiscoverSchemas(request);

            // assert
            Assert.IsType<DiscoverSchemasResponse>(response);
            Assert.Equal(1, response.Schemas.Count);

            var schema = response.Schemas[0];
            Assert.Equal($"AllClaims", schema.Id);
            Assert.Equal("AllClaims", schema.Name);
            Assert.Equal($"", schema.Query);
            Assert.Equal(10, schema.Sample.Count);
            Assert.Equal(30, schema.Properties.Count);

            var property = schema.Properties[0];
            Assert.Equal("ScriptNumber", property.Id);
            Assert.Equal("ScriptNumber", property.Name);
            Assert.Equal("", property.Description);
            Assert.Equal(PropertyType.String, property.Type);
            Assert.True(property.IsKey);
            Assert.False(property.IsNullable);

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }

        [Fact]
        public async Task ReadStreamTest()
        {
            // setup
            Server server = new Server
            {
                Services = { Publisher.BindService(new PluginLiberty.Plugin.Plugin()) },
                Ports = { new ServerPort("localhost", 0, ServerCredentials.Insecure) }
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var schema = GetTestSchema();

            var connectRequest = GetConnectSettings();

            var schemaRequest = new DiscoverSchemasRequest
            {
                Mode = DiscoverSchemasRequest.Types.Mode.Refresh,
                ToRefresh = { schema }
            };

            var request = new ReadRequest()
            {
                DataVersions = new DataVersions
                {
                    JobId = "test"
                },
                JobId = "test",
            };

            // act
            client.Connect(connectRequest);
            var schemasResponse = client.DiscoverSchemas(schemaRequest);
            request.Schema = schemasResponse.Schemas[0];

            var response = client.ReadStream(request);
            var responseStream = response.ResponseStream;
            var records = new List<Record>();

            while (await responseStream.MoveNext())
            {
                records.Add(responseStream.Current);
            }

            // assert
            // Assert.Equal(12765, records.Count);

            //var record = JsonConvert.DeserializeObject<Dictionary<string, object>>(records[0].DataJson);
            //Assert.Equal("6001776", record["ScriptNumber"]);
            //Assert.Equal("2018-05-04", record["WrittenDate"]);
            //Assert.Equal("3", record["RefillsAuthorized"]);
           //Assert.Equal("null", record["StatusCode"]);

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }

        [Fact]
        public async Task ReadStreamLimitTest()
        {
            // setup
            Server server = new Server
            {
                Services = { Publisher.BindService(new PluginLiberty.Plugin.Plugin()) },
                Ports = { new ServerPort("localhost", 0, ServerCredentials.Insecure) }
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var schema = GetTestSchema("AllClaims", "AllClaims", "AllClaims");

            var connectRequest = GetConnectSettings();

            var schemaRequest = new DiscoverSchemasRequest
            {
                Mode = DiscoverSchemasRequest.Types.Mode.Refresh,
                ToRefresh = { schema }
            };

            var request = new ReadRequest()
            {
                DataVersions = new DataVersions
                {
                    JobId = "test"
                },
                JobId = "test",
                Limit = 1
            };

            // act
            client.Connect(connectRequest);
            var schemasResponse = client.DiscoverSchemas(schemaRequest);
            request.Schema = schemasResponse.Schemas[0];

            var response = client.ReadStream(request);
            var responseStream = response.ResponseStream;
            var records = new List<Record>();

            while (await responseStream.MoveNext())
            {
                records.Add(responseStream.Current);
            }

            // assert
            Assert.Equal(1, records.Count);

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }
    }
}