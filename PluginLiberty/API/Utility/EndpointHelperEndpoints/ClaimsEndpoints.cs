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
    public class ClaimsEndpointHelper
    {
        private class ClaimsEndpoint : Endpoint
        {
            public string ScriptRefillPath { get; set; } = "";
            public string ClaimsPath { get; set; } = "";

            public override async Task<Schema> GetStaticSchemaAsync(IApiClient apiClient, Schema schema)
            {
                List<string> staticSchemaProperties = new List<string>()
                {
                    "ScriptNumber",
                    "RefillNumber",
                    "DateSubmitted",
                    "Coverage",
                    "Type",
                    "Status",
                    "Message",
                    "AuthNumber",
                    "PayorId",
                    "BIN",
                    "PCN",
                    "DrugId",
                    "RequestedACQ",
                    "RequestedCost",
                    "RequestedServiceFee",
                    "RequestedDispensingFee",
                    "RequestedCopay",
                    "RequestedTax",
                    "RequestedIncentive",
                    "RequestedUC",
                    "RequestedTotal",
                    "BasisOfCost",
                    "RepliedCost",
                    "RepliedServiceFee",
                    "RepliedDispensingFee",
                    "RepliedCopay",
                    "RepliedTax",
                    "RepliedIncentive",
                    "RepliedTotal",
                    "BasisOfReimbursement",
                    "OtherPayorAmt"
                };
                var properties = new List<Property>();
                foreach (var staticProperty in staticSchemaProperties)
                {
                    var property = new Property();
                    property.Id = staticProperty;
                    property.Name = staticProperty;

                    switch (staticProperty)
                    {
                        case ("ScriptNumber"):
                            property.Type = PropertyType.Integer;
                            property.IsKey = true;
                            property.TypeAtSource = "integer";
                            break;
                        case ("RefillNumber"):
                            property.Type = PropertyType.Integer;
                            property.IsKey = false;
                            property.TypeAtSource = "integer";
                            break;
                        case ("DateSubmitted"):
                        case ("Coverage"):
                        case ("Type"):
                        case ("Status"):
                        case ("Message"):
                        case ("AuthNumber"):
                        case ("PayorId"):
                        case ("BIN"):
                        case ("PCN"):
                        case ("DrugId"):
                        case ("BasisOfCost"):
                        case ("BasisOfReimbursement"):
                            property.Type = PropertyType.String;
                            property.IsKey = false;
                            property.TypeAtSource = "string";
                            break;
                        case ("RequestedACQ"):
                        case ("RequestedCost"):
                        case ("RequestedServiceFee"):
                        case ("RequestedDispensingFee"):
                        case ("RequestedCopay"):
                        case ("RequestedTax"):
                        case ("RequestedIncentive"):
                        case ("RequestedUC"):
                        case ("RequestedTotal"):
                        case ("RepliedCost"):
                        case ("RepliedServiceFee"):
                        case ("RepliedDispensingFee"):
                        case ("RepliedCopay"):
                        case ("RepliedTax"):
                        case ("RepliedIncentive"):
                        case ("RepliedTotal"):
                        case ("OtherPayorAmt"):
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
                var pageNumber = 1;
                bool hasMore;

                do
                {
                    // get all prescriptions
                    var prescriptionRetryCounter = 1;
                    bool retryPrescriptions;
                    HttpResponseMessage presciptionsResponse;

                    do
                    {
                        retryPrescriptions = false;
                        presciptionsResponse =
                            await apiClient.GetAsync(
                                $"/prescriptions?PageSize=100&StartDate={queryDate}&Page={pageNumber}");

                        if (!presciptionsResponse.IsSuccessStatusCode)
                        {
                            var error = JsonConvert.DeserializeObject<ApiError>(await presciptionsResponse.Content
                                .ReadAsStringAsync());
                            var ex = new Exception(error.Message);
                            Logger.Error(ex, "Prescriptions Retry Failed");
                            Thread.Sleep(prescriptionRetryCounter * prescriptionRetryCounter * 1000);
                            prescriptionRetryCounter++;
                            retryPrescriptions = prescriptionRetryCounter < 6;

                            if (!retryPrescriptions)
                            {
                                throw ex;
                            }
                        }
                    } while (retryPrescriptions);

                    var objectResponseWrapper =
                        JsonConvert.DeserializeObject<PrescriptionResponseWrapper>(
                            await presciptionsResponse.Content.ReadAsStringAsync());

                    if (objectResponseWrapper?.Scripts.Count == 0)
                    {
                        yield break;
                    }

                    foreach (var prescription in objectResponseWrapper?.Scripts)
                    {
                        if (Int32.Parse(prescription["ScriptNumber"].ToString()) != 0)
                        {
                            var scriptNumber = Int32.Parse(prescription["ScriptNumber"].ToString());
                            var lastRefillNumber = Int32.Parse(prescription["LastRefillNumber"].ToString());

                            for (var refillNumber = 0; refillNumber < lastRefillNumber + 1; refillNumber++)
                            {
                                // get claim
                                var claimRetryCounter = 1;
                                bool retryClaim;
                                HttpResponseMessage claimResponse;

                                do
                                {
                                    retryClaim = false;
                                    claimResponse =
                                        await apiClient.GetAsync($"{ClaimsPath}/{scriptNumber}/{refillNumber}");

                                    if (claimResponse.StatusCode == HttpStatusCode.NotFound)
                                    {
                                        continue;
                                    }

                                    if (!claimResponse.IsSuccessStatusCode)
                                    {
                                        var error = JsonConvert.DeserializeObject<ApiError>(await claimResponse.Content
                                            .ReadAsStringAsync());
                                        var ex = new Exception(error.Message);
                                        Logger.Error(ex, "Claim Retry Failed");
                                        Thread.Sleep(claimRetryCounter * claimRetryCounter * 1000);
                                        claimRetryCounter++;
                                        retryClaim = claimRetryCounter < 6;

                                        if (!retryClaim)
                                        {
                                            throw ex;
                                        }
                                    }
                                } while (retryClaim);

                                if (claimResponse.IsSuccessStatusCode)
                                {
                                    var raw = await claimResponse.Content.ReadAsStringAsync();
                                    var claimsPropertiesResponse =
                                        JsonConvert.DeserializeObject<List<ClaimsWrapper>>(
                                            await claimResponse.Content.ReadAsStringAsync());

                                    foreach (var claim in claimsPropertiesResponse)
                                    {
                                        var recordMap = new Dictionary<string, object>();


                                        try
                                        {
                                            recordMap["ScriptNumber"] = claim.ScriptNumber ?? null;
                                            recordMap["RefillNumber"] = claim.RefillNumber ?? null;
                                            recordMap["DateSubmitted"] = claim.DateSubmitted ?? null;
                                            recordMap["Coverage"] = claim.Coverage ?? null;
                                            recordMap["Type"] = claim.Type ?? null;
                                            recordMap["Status"] = claim.Status ?? null;
                                            recordMap["Message"] = claim.Message ?? null;
                                            recordMap["AuthNumber"] = claim.AuthNumber ?? null;
                                            recordMap["PayorId"] = claim.PayorId ?? null;
                                            recordMap["BIN"] = claim.BIN ?? null;
                                            recordMap["PCN"] = claim.PCN ?? null;
                                            recordMap["DrugId"] = claim.DrugId ?? null;
                                            recordMap["RequestedACQ"] = claim.RequestedACQ;
                                            recordMap["RequestedCost"] = claim.RequestedCost;
                                            recordMap["RequestedServiceFee"] = claim.RequestedServiceFee;
                                            recordMap["RequestedDispensingFee"] =
                                                claim.RequestedDispensingFee;
                                            recordMap["RequestedCopay"] = claim.RequestedCopay;
                                            recordMap["RequestedTax"] = claim.RequestedTax;
                                            recordMap["RequestedIncentive"] = claim.RequestedIncentive;
                                            recordMap["RequestedUC"] = claim.RequestedUC;
                                            recordMap["RequestedTotal"] = claim.RequestedTotal;
                                            recordMap["BasisOfCost"] = claim.BasisOfCost ?? null;
                                            recordMap["RepliedCost"] = claim.RepliedCost;
                                            recordMap["RepliedServiceFee"] = claim.RepliedServiceFee;
                                            recordMap["RepliedDispensingFee"] =
                                                claim.RepliedDispensingFee;
                                            recordMap["RepliedCopay"] = claim.RepliedCopay;
                                            recordMap["RepliedTax"] = claim.RepliedTax;
                                            recordMap["RepliedIncentive"] = claim.RepliedIncentive;
                                            recordMap["RepliedTotal"] = claim.RepliedTotal;
                                            recordMap["BasisOfReimbursement"] =
                                                claim.BasisOfReimbursement ?? null;
                                            recordMap["OtherPayorAmt"] = claim.OtherPayorAmt;
                                        }
                                        catch (Exception e)
                                        {
                                            var debug = e.Message;
                                        }

                                        yield return new Record
                                        {
                                            Action = Record.Types.Action.Upsert,
                                            DataJson = JsonConvert.SerializeObject(recordMap)
                                        };
                                    }
                                }
                            }
                        }
                    }

                    var page = objectResponseWrapper.Page;
                    var recordCount = objectResponseWrapper.RecordCount;
                    var pageSize = objectResponseWrapper.PageSize;

                    hasMore = page * pageSize <= recordCount;
                    pageNumber++;
                } while (hasMore);
            }
        }

        public static readonly Dictionary<string, Endpoint> ClaimsEndpoints =
            new Dictionary<string, Endpoint>
            {
                {
                    "AllClaims", new ClaimsEndpoint
                    {
                        Id = "AllClaims",
                        ShouldGetStaticSchema = true,
                        Name = "AllClaims",
                        BasePath = "https://api.libertysoftware.com",
                        ScriptRefillPath = $"/prescriptions",
                        ClaimsPath = $"/claims",
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