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
                            property.Type = PropertyType.String;
                            property.IsKey = true;
                            property.TypeAtSource = "string";
                            break;
                        case ("RefillNumber"):
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
                            property.Type = PropertyType.String;
                            property.IsKey = false;
                            property.TypeAtSource = "string";
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
                                            recordMap["ScriptNumber"] = claim.ScriptNumber != null
                                                ? claim.ScriptNumber.ToString()
                                                : "null";
                                            recordMap["RefillNumber"] = claim.RefillNumber != null
                                                ? claim.RefillNumber.ToString()
                                                : "null";
                                            recordMap["DateSubmitted"] = claim.DateSubmitted != null
                                                ? claim.DateSubmitted.ToString()
                                                : "null";
                                            recordMap["Coverage"] = claim.Coverage != null
                                                ? claim.Coverage.ToString()
                                                : "null";
                                            recordMap["Type"] = claim.Type != null
                                                ? claim.Type.ToString()
                                                : "null";
                                            recordMap["Status"] = claim.Status!= null
                                                ? claim.Status.ToString()
                                                : "null";
                                            recordMap["Message"] = claim.Message != null
                                                ? claim.Message.ToString()
                                                : "null";
                                            recordMap["AuthNumber"] = claim.AuthNumber != null
                                                ? claim.AuthNumber.ToString()
                                                : "null";
                                            recordMap["PayorId"] = claim.PayorId != null
                                                ? claim.PayorId.ToString()
                                                : "null";
                                            recordMap["BIN"] = claim.BIN != null
                                                ? claim.BIN.ToString()
                                                : "null";
                                            recordMap["PCN"] = claim.PCN != null
                                                ? claim.PCN.ToString()
                                                : "null";
                                            recordMap["DrugId"] = claim.DrugId != null
                                                ? claim.DrugId.ToString()
                                                : "null";
                                            recordMap["RequestedACQ"] = claim.RequestedACQ != null
                                                ? claim.RequestedACQ.ToString()
                                                : "null";
                                            recordMap["RequestedCost"] = claim.RequestedCost!= null
                                                ? claim.RequestedCost.ToString()
                                                : "null";
                                            recordMap["RequestedServiceFee"] = claim.RequestedServiceFee != null
                                                ? claim.RequestedServiceFee.ToString()
                                                : "null";
                                            recordMap["RequestedDispensingFee"] = claim.RequestedDispensingFee != null
                                                ? claim.RequestedDispensingFee.ToString()
                                                : "null";
                                            recordMap["RequestedCopay"] = claim.RequestedCopay != null
                                                ? claim.RequestedCopay.ToString()
                                                : "null";
                                            recordMap["RequestedTax"] = claim.RequestedTax != null
                                                ? claim.RequestedTax.ToString()
                                                : "null";
                                            recordMap["RequestedIncentive"] = claim.RequestedIncentive != null
                                                ? claim.RequestedIncentive.ToString()
                                                : "null";
                                            recordMap["RequestedUC"] = claim.RequestedUC != null
                                                ? claim.RequestedUC.ToString()
                                                : "null";
                                            recordMap["RequestedTotal"] = claim.RequestedTotal!= null
                                                ? claim.RequestedTotal.ToString()
                                                : "null";
                                            recordMap["BasisOfCost"] = claim.BasisOfCost != null
                                                ? claim.BasisOfCost.ToString()
                                                : "null";
                                            recordMap["RepliedCost"] = claim.RepliedCost != null
                                                ? claim.RepliedCost.ToString()
                                                : "null";
                                            recordMap["RepliedServiceFee"] = claim.RepliedServiceFee != null
                                                ? claim.RepliedServiceFee.ToString()
                                                : "null";
                                            recordMap["RepliedDispensingFee"] = claim.RepliedDispensingFee != null
                                                ? claim.RepliedDispensingFee.ToString()
                                                : "null";
                                            recordMap["RepliedCopay"] = claim.RepliedCopay!= null
                                                ? claim.RepliedCopay.ToString()
                                                : "null";
                                            recordMap["RepliedTax"] = claim.RepliedTax != null
                                                ? claim.RepliedTax.ToString()
                                                : "null";
                                            recordMap["RepliedIncentive"] = claim.RepliedIncentive!= null
                                                ? claim.RepliedIncentive.ToString()
                                                : "null";
                                            recordMap["RepliedTotal"] = claim.RepliedTotal != null
                                                ? claim.RepliedTotal.ToString()
                                                : "null";
                                            recordMap["BasisOfReimbursement"] = claim.BasisOfReimbursement != null
                                                ? claim.BasisOfReimbursement.ToString()
                                                : "null";
                                            recordMap["OtherPayorAmt"] = claim.OtherPayorAmt != null
                                                ? claim.OtherPayorAmt.ToString()
                                                : "null";
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