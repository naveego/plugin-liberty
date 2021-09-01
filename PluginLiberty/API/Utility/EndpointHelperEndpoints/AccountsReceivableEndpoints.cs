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
                        case ("OwnerPatientID"):
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

            public override async IAsyncEnumerable<Record> ReadRecordsAsync(IApiClient apiClient, Schema schema, bool isDiscoverRead = false)
        {
            var queryDate = DateTime.Parse(await apiClient.GetQueryDate());
            //while(queryDate <= DateTime.Now)
            {
                var patientRetryCounter = 1;
                //var response = await apiClient.GetAsync($"{PatientPath}&arOnly=1&modifiedStart={queryDate}");
                var response = await apiClient.GetAsync($"{PatientPath}&modifiedStart={queryDate}");
                var pageNumber = 1;

                if (!response.IsSuccessStatusCode)
                {
                    var error = JsonConvert.DeserializeObject<ApiError>(await response.Content.ReadAsStringAsync());
                    while (patientRetryCounter < 6)
                    {
                        response = await apiClient.GetAsync($"{PatientPath}&modifiedStart={queryDate}");
                        Thread.Sleep(patientRetryCounter*patientRetryCounter*1000);
                        patientRetryCounter++;
                        Logger.Error(new Exception(error.Message),"Patient Retry Failed");
                        if (response.IsSuccessStatusCode)
                        {
                            break;
                        }
                    }
                    throw new Exception(error.Message);
                }

                var objectPropertiesResponse =
                    JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(
                        await response.Content.ReadAsStringAsync());
                
                foreach (var patient in objectPropertiesResponse)
                {
                    if (Int32.Parse(patient["AccountNumber"].ToString()) != 0)
                    {
                        var ARresponse = await apiClient.GetAsync($"/ar/{patient["Id"].ToString()}");

                        if (!ARresponse.IsSuccessStatusCode)
                        {
                            var error = JsonConvert.DeserializeObject<ApiError>(await ARresponse.Content.ReadAsStringAsync());
                            var arRetryCounter = 1;
                            
                            while (arRetryCounter < 6)
                            {
                                ARresponse = await apiClient.GetAsync($"/ar/{patient["Id"].ToString()}");
                                Thread.Sleep(arRetryCounter*arRetryCounter*1000);
                                arRetryCounter++;
                                Logger.Error(new Exception(error.Message),"AR Retry Failed");
                                if (ARresponse.IsSuccessStatusCode)
                                {
                                    break;
                                }
                            }
                            throw new Exception(error.Message);
                        }      

                        var arPropertiesResponse =
                            JsonConvert.DeserializeObject<AccountsReceivableWrapper>(
                                await ARresponse.Content.ReadAsStringAsync());

                        var recordMap = new Dictionary<string, object>();   

                        recordMap["OwnerPatientId"] = arPropertiesResponse.OwnerPatientId ?? "";
                        recordMap["AccountNumber"] = arPropertiesResponse.AccountNumber;
                        recordMap["ChargeCode"] = arPropertiesResponse.ChargeCode?? "";
                        recordMap["CreditLimit"] = arPropertiesResponse.CreditLimit;
                        recordMap["LastPaymentAmount"] = arPropertiesResponse.LastPaymentAmount;
                        recordMap["LastPaymentDate"] = arPropertiesResponse.LastPaymentDate ?? "";
                        recordMap["PreviousBalance"] = arPropertiesResponse.PreviousBalance;
                        recordMap["AmountDue"] = arPropertiesResponse.AmountDue;
                        recordMap["Over30Days"] = arPropertiesResponse.Over30Days;
                        recordMap["Over60Days"] = arPropertiesResponse.Over60Days;
                        recordMap["Over90Days"] = arPropertiesResponse.Over90Days;
                        recordMap["Over120Days"] = arPropertiesResponse.Over120Days;
                        recordMap["CurrentDebits"] = arPropertiesResponse.CurrentDebits;
                        recordMap["CurrentCredits"] = arPropertiesResponse.CurrentCredits;
                        recordMap["TotalBalance"] = arPropertiesResponse.TotalBalance;

                        yield return new Record
                        {
                            Action = Record.Types.Action.Upsert,
                            DataJson = JsonConvert.SerializeObject(recordMap)
                        };
                    }
                }
               // queryDate = queryDate.AddDays(1);
            }
                // if (objectResponseWrapper?.Patient.Count == 0)
                // {
                //     yield break;
                // }
                // else
                // {

                    
                    // foreach (var objectResponse in objectResponseWrapper?.Patient)
                    // {
                    //     var recordMap = new Dictionary<string, object>();

                    //     foreach (var objectProperty in objectResponse)
                    //     {
                    //         switch(objectProperty.Key)
                            
                    //         {
                    //         case("Patient"):
                    //             try
                    //             { 
                    //                 var patientJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(objectProperty.Value.ToString());
                    //                 recordMap["PatientId"] = patientJson["Id"] ?? "";
                    //             }
                    //             catch
                    //             {
                    //                 recordMap[objectProperty.Key] = "";
                    //             }            
                    //             break;

                    //         default:

                    //             try
                    //             {
                    //                 recordMap[objectProperty.Key] = objectProperty.Value.ToString() ?? "";
                    //             }
                    //             catch
                    //             {
                    //                 recordMap[objectProperty.Key] = "";
                    //             }
                    //             break;

                    //         }
                    //     }  

                    //     yield return new Record
                    //     {
                    //         Action = Record.Types.Action.Upsert,
                    //         DataJson = JsonConvert.SerializeObject(recordMap)
                    //     };
                    // }
                    // pageNumber=pageNumber+1;
                }
            
    }
    

        public static readonly Dictionary<string, Endpoint> AccountsReceivableEndpoints = new Dictionary<string, Endpoint>
        {
            {
                "AllAccountsReceivable", new AccountsReceivableEndpoint
                {
                    Id = "AllAccountsReceivable",
                    ShouldGetStaticSchema = true,
                    Name = "AllAccountsReceivable",
                    BasePath = "https://api.libertysoftware.com",
                    PatientPath = $"/patient?PageSize=100",
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