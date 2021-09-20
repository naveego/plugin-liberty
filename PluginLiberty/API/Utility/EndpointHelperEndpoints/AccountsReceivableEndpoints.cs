using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Grpc.Core;
using Naveego.Sdk.Logging;
using Naveego.Sdk.Plugins;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PluginLiberty.API.Factory;
using PluginLiberty.DataContracts;

namespace PluginLiberty.API.Utility.EndpointHelperEndpoints
{
    public class AccountsReceivableEndpointHelper
    {
        private class AccountsReceivableEndpoint : Endpoint
        {
            public string PatientPath { get; set; } = "";

            public override async Task<Schema> GetStaticSchemaAsync(IApiClient apiClient, Schema schema)
            {
                List<string> staticSchemaProperties = new List<string>()
                {
                    "OwnerPatientId",
                    "AccountNumber",
                    "ChargeCode",
                    "CreditLimit",
                    "LastPaymentAmount",
                    "LastPaymentDate",
                    "PreviousBalance",
                    "AmountDue",
                    "Over30Days",
                    "Over60Days",
                    "Over90Days",
                    "Over120Days",
                    "CurrentDebits",
                    "CurrentCredits",
                    "TotalBalance"
                };
                var properties = new List<Property>();
                foreach (var staticProperty in staticSchemaProperties)
                {
                    var property = new Property();
                    property.Id = staticProperty;
                    property.Name = staticProperty;

                    switch (staticProperty)
                    {
                        case ("AccountNumber"):
                            property.Type = PropertyType.Integer;
                            property.IsKey = false;
                            property.TypeAtSource = "integer";
                            break;
                        case ("OwnerPatientId"):
                            property.Type = PropertyType.String;
                            property.IsKey = true;
                            property.TypeAtSource = "string";
                            break;
                        case ("LastPaymentDate"):
                        case ("ChargeCode"):
                            property.Type = PropertyType.String;
                            property.IsKey = false;
                            property.TypeAtSource = "string";
                            break;
                        case ("CreditLimit"):
                        case ("LastPaymentAmount"):
                        case ("PreviousBalance"):
                        case ("AmountDue"):
                        case ("Over30Days"):
                        case ("Over60Days"):
                        case ("Over90Days"):
                        case ("Over120Days"):
                        case ("CurrentDebits"):
                        case ("CurrentCredits"):
                        case ("TotalBalance"):
                            property.Type = PropertyType.Float;
                            property.IsKey = false;
                            property.TypeAtSource = "float";
                            break;
                        default:
                            property.IsKey = false;
                            property.TypeAtSource = "string";
                            break;
                    }

                    properties.Add(property);
                }

                schema.Properties.Clear();
                schema.Properties.AddRange(properties);
                schema.DataFlowDirection = GetDataFlowDirection();
                return schema;
            }

            public override async IAsyncEnumerable<Record> ReadRecordsAsync(IApiClient apiClient, Schema schema,
                bool isDiscoverRead = false)
            {
                var queryDate = DateTime.Parse(apiClient.GetQueryDate());
                bool hasMore;
                DateTime mostRecentDate;

                do
                {
                    // get all patients modified since query date (up to 1001 patients per API documentation)
                    var patientRetryCounter = 1;
                    bool retryPatient;
                    HttpResponseMessage patientResponse;

                    do
                    {
                        retryPatient = false;
                        patientResponse = await apiClient.GetAsync($"{PatientPath}?modifiedStart={queryDate}&arOnly=1");

                        if (!patientResponse.IsSuccessStatusCode)
                        {
                            var error = JsonConvert.DeserializeObject<ApiError>(await patientResponse.Content
                                .ReadAsStringAsync());
                            var ex = new Exception(error.Message);
                            Logger.Error(ex, "Patient Retry Failed");
                            Thread.Sleep(patientRetryCounter * patientRetryCounter * 1000);
                            patientRetryCounter++;
                            retryPatient = patientRetryCounter < 6;

                            if (!retryPatient)
                            {
                                throw ex;
                            }
                        }
                    } while (retryPatient);

                    var objectPropertiesResponse =
                        JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(
                            await patientResponse.Content.ReadAsStringAsync());

                    // find all patients that have AR data
                    foreach (var patient in objectPropertiesResponse)
                    {
                        // check if patient has AR data
                        if (Int32.Parse(patient["AccountNumber"].ToString()) != 0)
                        {
                            // get AR data for patient
                            var arRetryCounter = 1;
                            bool retryAR;
                            HttpResponseMessage arResponse;

                            do
                            {
                                retryAR = false;
                                arResponse = await apiClient.GetAsync($"/ar/{patient["Id"].ToString()}");

                                if (!arResponse.IsSuccessStatusCode)
                                {
                                    var error = JsonConvert.DeserializeObject<ApiError>(
                                        await arResponse.Content.ReadAsStringAsync());
                                    var ex = new Exception(error.Message);
                                    Logger.Error(ex, "AR Retry Failed");
                                    Thread.Sleep(arRetryCounter * arRetryCounter * 1000);
                                    arRetryCounter++;
                                    retryAR = arRetryCounter < 6;

                                    if (!retryAR)
                                    {
                                        throw ex;
                                    }
                                }
                            } while (retryAR);

                            var arPropertiesResponse =
                                JsonConvert.DeserializeObject<AccountsReceivableWrapper>(
                                    await arResponse.Content.ReadAsStringAsync());

                            // build patient AR data record
                            var recordMap = new Dictionary<string, object>();

                            recordMap["OwnerPatientId"] = arPropertiesResponse.OwnerPatientId ?? "";
                            recordMap["AccountNumber"] = arPropertiesResponse.AccountNumber;
                            recordMap["ChargeCode"] = arPropertiesResponse.ChargeCode;
                            recordMap["CreditLimit"] = arPropertiesResponse.CreditLimit;
                            recordMap["LastPaymentAmount"] = arPropertiesResponse.LastPaymentAmount;
                            recordMap["LastPaymentDate"] = arPropertiesResponse.LastPaymentDate;
                            recordMap["PreviousBalance"] = arPropertiesResponse.PreviousBalance;
                            recordMap["AmountDue"] = arPropertiesResponse.AmountDue;
                            recordMap["Over30Days"] = arPropertiesResponse.Over30Days;
                            recordMap["Over60Days"] = arPropertiesResponse.Over60Days;
                            recordMap["Over90Days"] = arPropertiesResponse.Over90Days;
                            recordMap["Over120Days"] = arPropertiesResponse.Over120Days;
                            recordMap["CurrentDebits"] = arPropertiesResponse.CurrentDebits;
                            recordMap["CurrentCredits"] = arPropertiesResponse.CurrentCredits;
                            recordMap["TotalBalance"] = arPropertiesResponse.TotalBalance;

                            // return patient AR data record
                            yield return new Record
                            {
                                Action = Record.Types.Action.Upsert,
                                DataJson = JsonConvert.SerializeObject(recordMap)
                            };
                        }
                    }

                    // TODO: Handle when there are 1001 records as that is the api limit
                    // hasMore = objectPropertiesResponse.Count == 1001;
                    hasMore = false;
                } while (hasMore);
            }
        }

        public static readonly Dictionary<string, Endpoint> AccountsReceivableEndpoints =
            new Dictionary<string, Endpoint>
            {
                {
                    "AllAccountsReceivable", new AccountsReceivableEndpoint
                    {
                        Id = "AllAccountsReceivable",
                        ShouldGetStaticSchema = true,
                        Name = "AllAccountsReceivable",
                        BasePath = "https://api.libertysoftware.com",
                        PatientPath = $"/patient",
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