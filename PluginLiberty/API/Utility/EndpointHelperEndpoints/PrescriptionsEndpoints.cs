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
    public class PrescriptionsEndpointHelper
    {
        private class PrescriptionsEndpoint : Endpoint
        {
            public override async Task<Schema> GetStaticSchemaAsync(IApiClient apiClient, Schema schema)
            {
                List<string> staticSchemaProperties = new List<string>()
                {
                    "ScriptNumber",
                    "WrittenDate",
                    "RefillsAuthorized",
                    "LastRefillNumber",
                    "RefillUntilDate",
                    "FullDispenseQuantity",
                    "AuthorizedQuantity",
                    "AvailableQuantity",
                    "Origin",
                    "QueueName",
                    "IsAutoFill",
                    "Location",
                    "Status",
                    "StatusCode",
                    "Patient.Id",
                    "Patient.ExternalId",
                    "Patient.AccountNumber",
                    "Patient.ChargeCode",
                    "Patient.Birthdate",
                    "Patient.Gender",
                    "Patient.SSN",
                    "Patient.Phone",
                    "Patient.PhoneType",
                    "Patient.Phone2",
                    "Patient.Phone2Type",
                    "Patient.IsTextOk",
                    "Patient.Email",
                    "Patient.Language",
                    "Patient.CustomField1",
                    "Patient.CustomField2",
                    "Patient.CustomField3",
                    "Patient.CustomField4",
                    "Patient.Allergies",
                    "Patient.NursingHome",
                    "Patient.FirstName",
                    "Patient.MiddleInitial",
                    "Patient.LastName",
                    "Patient.Street1",
                    "Patient.Street2",
                    "Patient.City",
                    "Patient.State",
                    "Patient.Zip",
                    "DrugPescribed.Id",
                    "DrugPescribed.NDC",
                    "DrugPescribed.Name",
                    "DrugPescribed.IsCompound",
                    "DrugPescribed.IsVaccine",
                    "DrugPescribed.Manufacturer",
                    "DrugPescribed.Schedule",
                    "DrugPescribed.Imprint",
                    "DrugPescribed.Warnings",
                    "DrugPescribed.CustomField1",
                    "DrugPescribed.CustomField2",
                    "DrugPescribed.CustomField3",
                    "DrugPescribed.CustomField4",
                    "DrugPescribed.Strength",
                    "Prescriber.Id",
                    "Prescriber.Phone",
                    "Prescriber.Fax",
                    "Prescriber.NPI",
                    "Prescriber.DEA",
                    "Prescriber.CustomField1",
                    "Prescriber.CustomField2",
                    "Prescriber.CustomField3",
                    "Prescriber.CustomField4",
                    "Prescriber.FirstName",
                    "Prescriber.MiddleInitial",
                    "Prescriber.LastName",
                    "Prescriber.Street1",
                    "Prescriber.Street2",
                    "Prescriber.City",
                    "Prescriber.State",
                    "Prescriber.Zip",
                    "Fill.RefillNumber",
                    "Fill.DispenseDate",
                    "Fill.DispenseQuantity",
                    "Fill.DaysSupply",
                    "Fill.SIG",
                    "Fill.DAW",
                    "Fill.RphInitials",
                    "Fill.RphName",
                    "Fill.Status",
                    "Fill.StatusCode",
                    "Fill.PatientPay",
                    "Fill.ExpirationDate",
                    "Fill.LotNumber",
                    "Fill.WorfkflowLocation",
                    "Fill.Cost",
                    "Fill.ACQ",
                    "Fill.AWP",
                    "Fill.UsualAndCustomary",
                    "Fill.Doses",
                    "Fill.PrimaryId",
                    "Fill.PrimaryName",
                    "Fill.PrimaryBIN",
                    "Fill.PrimaryPCN",
                    "Fill.PrimaryInsurancePay",
                    "Fill.PrimaryPatientPay",
                    "Fill.SecondaryId",
                    "Fill.SecondaryName",
                    "Fill.SecondaryBIN",
                    "Fill.SecondaryPCN",
                    "Fill.SeocondaryInsurancePay",
                    "Fill.SecondaryPatientPay",
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
                        case ("WrittenDate"):
                        case ("RefillsAuthorized"):
                        case ("LastRefillNumber"):
                        case ("RefillUntilDate"):
                        case ("Origin"):
                        case ("QueueName"):
                        case ("IsAutoFill"):
                        case ("Location"):
                        case ("Status"):
                        case ("StatusCode"):
                        case ("Patient.Id"):
                        case ("Patient.ExternalId"):
                        case ("Patient.AccountNumber"):
                        case ("Patient.ChargeCode"):
                        case ("Patient.Birthdate"):
                        case ("Patient.Gender"):
                        case ("Patient.SSN"):
                        case ("Patient.Phone"):
                        case ("Patient.PhoneType"):
                        case ("Patient.Phone2"):
                        case ("Patient.Phone2Type"):
                        case ("Patient.IsTextOk"):
                        case ("Patient.Email"):
                        case ("Patient.Language"):
                        case ("Patient.CustomField1"):
                        case ("Patient.CustomField2"):
                        case ("Patient.CustomField3"):
                        case ("Patient.CustomField4"):
                        case ("Patient.Allergies"):
                        case ("Patient.NursingHome"):
                        case ("Patient.FirstName"):
                        case ("Patient.MiddleInitial"):
                        case ("Patient.LastName"):
                        case ("Patient.Street1"):
                        case ("Patient.Street2"):
                        case ("Patient.City"):
                        case ("Patient.State"):
                        case ("Patient.Zip"):
                        case ("DrugPescribed.Id"):
                        case ("DrugPescribed.NDC"):
                        case ("DrugPescribed.Name"):
                        case ("DrugPescribed.IsCompound"):
                        case ("DrugPescribed.IsVaccine"):
                        case ("DrugPescribed.Manufacturer"):
                        case ("DrugPescribed.Schedule"):
                        case ("DrugPescribed.Imprint"):
                        case ("DrugPescribed.Warnings"):
                        case ("DrugPescribed.CustomField1"):
                        case ("DrugPescribed.CustomField2"):
                        case ("DrugPescribed.CustomField3"):
                        case ("DrugPescribed.CustomField4"):
                        case ("DrugPescribed.Strength"):
                        case ("Prescriber.Id"):
                        case ("Prescriber.Phone"):
                        case ("Prescriber.Fax"):
                        case ("Prescriber.NPI"):
                        case ("Prescriber.DEA"):
                        case ("Prescriber.CustomField1"):
                        case ("Prescriber.CustomField2"):
                        case ("Prescriber.CustomField3"):
                        case ("Prescriber.CustomField4"):
                        case ("Prescriber.FirstName"):
                        case ("Prescriber.MiddleInitial"):
                        case ("Prescriber.LastName"):
                        case ("Prescriber.Street1"):
                        case ("Prescriber.Street2"):
                        case ("Prescriber.City"):
                        case ("Prescriber.State"):
                        case ("Prescriber.Zip"):
                        case ("Fill.DispenseDate"):
                        case ("Fill.SIG"):
                        case ("Fill.DAW"):
                        case ("Fill.RphInitials"):
                        case ("Fill.RphName"):
                        case ("Fill.Status"):
                        case ("Fill.StatusCode"):
                        case ("Fill.ExpirationDate"):
                        case ("Fill.LotNumber"):
                        case ("Fill.WorfkflowLocation"):
                        case ("Fill.PrimaryId"):
                        case ("Fill.PrimaryName"):
                        case ("Fill.PrimaryBIN"):
                        case ("Fill.PrimaryPCN"):
                        case ("Fill.SecondaryId"):
                        case ("Fill.SecondaryName"):
                        case ("Fill.SecondaryBIN"):
                        case ("Fill.SecondaryPCN"):
                            property.Type = PropertyType.String;
                            property.IsKey = false;
                            property.TypeAtSource = "string";
                            break;
                        case ("FullDispenseQuantity"):
                        case ("AuthorizedQuantity"):
                        case ("AvailableQuantity"):
                        case ("Fill.RefillNumber"):
                        case ("Fill.DispenseQuantity"):
                        case ("Fill.DaysSupply"):
                        case ("Fill.PatientPay"):
                        case ("Fill.ACQ"):
                        case ("Fill.AWP"):
                        case ("Fill.UsualAndCustomary"):
                        case ("Fill.Doses"):
                        case ("Fill.Cost"):
                        case ("Fill.SeocondaryInsurancePay"):
                        case ("Fill.SecondaryPatientPay"):
                        case ("Fill.PrimaryInsurancePay"):
                        case ("Fill.PrimaryPatientPay"):
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
                        presciptionsResponse = await apiClient.GetAsync($"/prescriptions?PageSize=100&StartDate={queryDate}&Page={pageNumber}");

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

                    foreach (var Script in objectResponseWrapper?.Scripts)
                    {
                        var recordMap = new Dictionary<string, object>();

                        recordMap["ScriptNumber"] = Script["ScriptNumber"] != null
                            ? Script["ScriptNumber"].ToString()
                            : "null";
                        recordMap["WrittenDate"] = Script["WrittenDate"] != null
                            ? Script["WrittenDate"].ToString()
                            : "null";
                        recordMap["RefillsAuthorized"] = Script["RefillsAuthorized"] != null
                            ? Script["RefillsAuthorized"].ToString()
                            : "null";
                        recordMap["LastRefillNumber"] = Script["LastRefillNumber"] != null
                            ? Script["LastRefillNumber"].ToString()
                            : "null";
                        recordMap["RefillUntilDate"] = Script["RefillUntilDate"] != null
                            ? Script["RefillUntilDate"].ToString()
                            : "null";
                        recordMap["FullDispenseQuantity"] = Script["FullDispenseQuantity"] != null
                            ? Script["FullDispenseQuantity"].ToString()
                            : "0";
                        recordMap["AuthorizedQuantity"] = Script["AuthorizedQuantity"] != null
                            ? Script["AuthorizedQuantity"].ToString()
                            : "0";
                        recordMap["AvailableQuantity"] = Script["AvailableQuantity"] != null
                            ? Script["AvailableQuantity"].ToString()
                            : "0";
                        recordMap["Origin"] =
                            Script["Origin"] != null ? Script["Origin"].ToString() : "null";
                        recordMap["QueueName"] = Script["QueueName"] != null
                            ? Script["QueueName"].ToString()
                            : "null";
                        recordMap["IsAutoFill"] = Script["IsAutoFill"] != null
                            ? Script["IsAutoFill"].ToString()
                            : "null";
                        recordMap["Location"] = Script["Location"] != null
                            ? Script["Location"].ToString()
                            : "null";
                        recordMap["Status"] =
                            Script["Status"] != null ? Script["Status"].ToString() : "null";
                        recordMap["StatusCode"] = Script["StatusCode"] != null
                            ? Script["StatusCode"].ToString()
                            : "null";


                        if (Script["Patient"] != null)
                        {
                            var thisPatient =
                                JsonConvert.DeserializeObject<Dictionary<string, object>>(
                                    Script["Patient"].ToString() ?? "{}");
                            recordMap["Patient.Id"] = thisPatient["Id"] != null
                                ? thisPatient["Id"].ToString()
                                : "null";
                            recordMap["Patient.ExternalId"] = thisPatient["ExternalId"] != null
                                ? thisPatient["ExternalId"].ToString()
                                : "null";
                            recordMap["Patient.AccountNumber"] = thisPatient["AccountNumber"] != null
                                ? thisPatient["AccountNumber"].ToString()
                                : "null";
                            recordMap["Patient.ChargeCode"] = thisPatient["ChargeCode"] != null
                                ? thisPatient["ChargeCode"].ToString()
                                : "null";
                            recordMap["Patient.Birthdate"] = thisPatient["BirthDate"] != null
                                ? thisPatient["BirthDate"].ToString()
                                : "null";
                            recordMap["Patient.Gender"] = thisPatient["Gender"] != null
                                ? thisPatient["Gender"].ToString()
                                : "null";
                            recordMap["Patient.SSN"] = thisPatient["SSN"] != null
                                ? thisPatient["SSN"].ToString()
                                : "null";
                            recordMap["Patient.Phone"] = thisPatient["Phone"] != null
                                ? thisPatient["Phone"].ToString()
                                : "null";
                            recordMap["Patient.PhoneType"] = thisPatient["PhoneType"] != null
                                ? thisPatient["PhoneType"].ToString()
                                : "null";
                            recordMap["Patient.Phone2"] = thisPatient["Phone2"] != null
                                ? thisPatient["Phone2"].ToString()
                                : "null";
                            recordMap["Patient.Phone2Type"] = thisPatient["Phone2Type"] != null
                                ? thisPatient["Phone2Type"].ToString()
                                : "null";
                            recordMap["Patient.IsTextOk"] = thisPatient["IsTextOk"] != null
                                ? thisPatient["IsTextOk"].ToString()
                                : "null";
                            recordMap["Patient.Email"] = thisPatient["Email"] != null
                                ? thisPatient["Email"].ToString()
                                : "null";
                            recordMap["Patient.Language"] = thisPatient["Language"] != null
                                ? thisPatient["Language"].ToString()
                                : "null";
                            recordMap["Patient.CustomField1"] = thisPatient["CustomField1"] != null
                                ? thisPatient["CustomField1"].ToString()
                                : "null";
                            recordMap["Patient.CustomField2"] = thisPatient["CustomField2"] != null
                                ? thisPatient["CustomField2"].ToString()
                                : "null";
                            recordMap["Patient.CustomField3"] = thisPatient["CustomField3"] != null
                                ? thisPatient["CustomField3"].ToString()
                                : "null";
                            recordMap["Patient.CustomField4"] = thisPatient["CustomField4"] != null
                                ? thisPatient["CustomField4"].ToString()
                                : "null";
                            recordMap["Patient.Allergies"] = thisPatient["Allergies"] != null
                                ? thisPatient["Allergies"].ToString()
                                : "null";
                            recordMap["Patient.NursingHome"] = thisPatient["NursingHome"] != null
                                ? thisPatient["NursingHome"].ToString()
                                : "null";

                            if (thisPatient["Name"] != null)
                            {
                                var thisPatientName =
                                    JsonConvert.DeserializeObject<Dictionary<string, object>>(
                                        thisPatient["Name"].ToString() ?? "{}");
                                recordMap["Patient.FirstName"] = thisPatientName["FirstName"] != null
                                    ? thisPatientName["FirstName"].ToString()
                                    : "null";
                                recordMap["PatientMiddleInitial"] = thisPatientName["MiddleInitial"] != null
                                    ? thisPatientName["MiddleInitial"].ToString()
                                    : "null";
                                recordMap["Patient.LastName"] = thisPatientName["LastName"] != null
                                    ? thisPatientName["LastName"].ToString()
                                    : "null";
                            }
                            else
                            {
                                recordMap["Patient.FirstName"] = "null";
                                recordMap["PatientMiddleInitial"] = "null";
                                recordMap["Patient.LastName"] = "null";
                            }

                            if (thisPatient["Address"] != null)
                            {
                                var thisPatientAddress =
                                    JsonConvert.DeserializeObject<Dictionary<string, object>>(
                                        thisPatient["Address"].ToString() ?? "{}");
                                recordMap["Patient.Street1"] = thisPatientAddress["Street1"] != null
                                    ? thisPatientAddress["Street1"].ToString()
                                    : "null";
                                recordMap["Patient.Street2"] = thisPatientAddress["Street2"] != null
                                    ? thisPatientAddress["Street2"].ToString()
                                    : "null";
                                recordMap["Patient.City"] = thisPatientAddress["City"] != null
                                    ? thisPatientAddress["City"].ToString()
                                    : "null";
                                recordMap["Patient.State"] = thisPatientAddress["State"] != null
                                    ? thisPatientAddress["State"].ToString()
                                    : "null";
                                recordMap["Patient.Zip"] = thisPatientAddress["Zip"] != null
                                    ? thisPatientAddress["Zip"].ToString()
                                    : "null";
                            }
                            else
                            {
                                recordMap["Patient.Street1"] = "null";
                                recordMap["Patient.Street2"] = "null";
                                recordMap["Patient.City"] = "null";
                                recordMap["Patient.State"] = "null";
                                recordMap["Patient.Zip"] = "null";
                            }
                        }
                        else
                        {
                            recordMap["Patient.Id"] = "null";
                            recordMap["Patient.ExternalId"] = "null";
                            recordMap["Patient.AccountNumber"] = "null";
                            recordMap["Patient.ChargeCode"] = "null";
                            recordMap["Patient.Birthdate"] = "null";
                            recordMap["Patient.Gender"] = "null";
                            recordMap["Patient.SSN"] = "null";
                            recordMap["Patient.Phone"] = "null";
                            recordMap["Patient.PhoneType"] = "null";
                            recordMap["Patient.Phone2"] = "null";
                            recordMap["Patient.Phone2Type"] = "null";
                            recordMap["Patient.IsTextOk"] = "null";
                            recordMap["Patient.Email"] = "null";
                            recordMap["Patient.Language"] = "null";
                            recordMap["Patient.CustomField1"] = "null";
                            recordMap["Patient.CustomField2"] = "null";
                            recordMap["Patient.CustomField3"] = "null";
                            recordMap["Patient.CustomField4"] = "null";
                            recordMap["Patient.Allergies"] = "null";
                            recordMap["Patient.NursingHome"] = "null";
                            recordMap["Patient.FirstName"] = "null";
                            recordMap["PatientMiddleInitial"] = "null";
                            recordMap["Patient.LastName"] = "null";
                            recordMap["Patient.Street1"] = "null";
                            recordMap["Patient.Street2"] = "null";
                            recordMap["Patient.City"] = "null";
                            recordMap["Patient.State"] = "null";
                            recordMap["Patient.Zip"] = "null";
                        }

                        if (Script["DrugPrescribed"] != null)
                        {
                            var thisDrugPrescribed =
                                JsonConvert.DeserializeObject<Dictionary<string, object>>(
                                    Script["DrugPrescribed"].ToString() ?? "{}");
                            recordMap["DrugPescribed.Id"] = thisDrugPrescribed["Id"] != null
                                ? thisDrugPrescribed["Id"].ToString()
                                : "null";
                            ;
                            recordMap["DrugPescribed.NDC"] = thisDrugPrescribed["NDC"] != null
                                ? thisDrugPrescribed["NDC"].ToString()
                                : "null";
                            recordMap["DrugPescribed.Name"] = thisDrugPrescribed["Name"] != null
                                ? thisDrugPrescribed["Name"].ToString()
                                : "null";
                            ;
                            recordMap["DrugPescribed.IsCompound"] = thisDrugPrescribed["IsCompound"] != null
                                ? thisDrugPrescribed["IsCompound"].ToString()
                                : "null";
                            recordMap["DrugPescribed.IsVaccine"] = thisDrugPrescribed["IsVaccine"] != null
                                ? thisDrugPrescribed["IsVaccine"].ToString()
                                : "null";
                            recordMap["DrugPescribed.Manufacturer"] =
                                thisDrugPrescribed["Manufacturer"] != null
                                    ? thisDrugPrescribed["Manufacturer"].ToString()
                                    : "null";
                            recordMap["DrugPescribed.Schedule"] = thisDrugPrescribed["Schedule"] != null
                                ? thisDrugPrescribed["Schedule"].ToString()
                                : "null";
                            recordMap["DrugPescribed.Imprint"] = thisDrugPrescribed["Imprint"] != null
                                ? thisDrugPrescribed["Imprint"].ToString()
                                : "null";
                            recordMap["DrugPescribed.Warnings"] = thisDrugPrescribed["Warnings"] != null
                                ? thisDrugPrescribed["Warnings"].ToString()
                                : "null";
                            recordMap["DrugPescribed.CustomField1"] =
                                thisDrugPrescribed["CustomField1"] != null
                                    ? thisDrugPrescribed["CustomField1"].ToString()
                                    : "null";
                            recordMap["DrugPescribed.CustomField2"] =
                                thisDrugPrescribed["CustomField2"] != null
                                    ? thisDrugPrescribed["CustomField2"].ToString()
                                    : "null";
                            recordMap["DrugPescribed.CustomField3"] =
                                thisDrugPrescribed["CustomField3"] != null
                                    ? thisDrugPrescribed["CustomField3"].ToString()
                                    : "null";
                            recordMap["DrugPescribed.CustomField4"] =
                                thisDrugPrescribed["CustomField4"] != null
                                    ? thisDrugPrescribed["CustomField4"].ToString()
                                    : "null";
                            recordMap["DrugPescribed.Strength"] = thisDrugPrescribed["Strength"] != null
                                ? thisDrugPrescribed["Strength"].ToString()
                                : "null";
                        }
                        else
                        {
                            recordMap["DrugPescribed.Id"] = "null";
                            recordMap["DrugPescribed.NDC"] = "null";
                            recordMap["DrugPescribed.Name"] = "null";
                            recordMap["DrugPescribed.IsCompound"] = "null";
                            recordMap["DrugPescribed.IsVaccine"] = "null";
                            recordMap["DrugPescribed.Manufacturer"] = "null";
                            recordMap["DrugPescribed.Schedule"] = "null";
                            recordMap["DrugPescribed.Imprint"] = "null";
                            recordMap["DrugPescribed.Warnings"] = "null";
                            recordMap["DrugPescribed.CustomField1"] = "null";
                            recordMap["DrugPescribed.CustomField2"] = "null";
                            recordMap["DrugPescribed.CustomField3"] = "null";
                            recordMap["DrugPescribed.CustomField4"] = "null";
                            recordMap["DrugPescribed.Strength"] = "null";
                        }

                        if (Script["Prescriber"] != null)
                        {
                            var thisPrescriber =
                                JsonConvert.DeserializeObject<Dictionary<string, object>>(
                                    Script["Prescriber"].ToString() ?? "{}");
                            recordMap["Prescriber.Id"] = thisPrescriber["Id"] != null
                                ? thisPrescriber["Id"].ToString()
                                : "null";
                            recordMap["Prescriber.Phone"] = thisPrescriber["Phone"] != null
                                ? thisPrescriber["Phone"].ToString()
                                : "null";
                            recordMap["Prescriber.Fax"] = thisPrescriber["Fax"] != null
                                ? thisPrescriber["Fax"].ToString()
                                : "null";
                            recordMap["Prescriber.NPI"] = thisPrescriber["NPI"] != null
                                ? thisPrescriber["NPI"].ToString()
                                : "null";
                            recordMap["Prescriber.DEA"] = thisPrescriber["DEA"] != null
                                ? thisPrescriber["DEA"].ToString()
                                : "null";
                            recordMap["Prescriber.CustomField1"] = thisPrescriber["CustomField1"] != null
                                ? thisPrescriber["CustomField1"].ToString()
                                : "null";
                            recordMap["Prescriber.CustomField2"] = thisPrescriber["CustomField2"] != null
                                ? thisPrescriber["CustomField2"].ToString()
                                : "null";
                            recordMap["Prescriber.CustomField3"] = thisPrescriber["CustomField3"] != null
                                ? thisPrescriber["CustomField3"].ToString()
                                : "null";
                            recordMap["Prescriber.CustomField4"] = thisPrescriber["CustomField4"] != null
                                ? thisPrescriber["CustomField4"].ToString()
                                : "null";

                            if (thisPrescriber["Name"] != null)
                            {
                                var thisPrescriberName =
                                    JsonConvert.DeserializeObject<Dictionary<string, object>>(
                                        thisPrescriber["Name"].ToString() ?? "{}");
                                recordMap["Prescriber.FirstName"] = thisPrescriberName["FirstName"] != null
                                    ? thisPrescriberName["FirstName"].ToString()
                                    : "null";
                                recordMap["Prescriber.MiddleInitial"] =
                                    thisPrescriberName["MiddleInitial"] != null
                                        ? thisPrescriberName["MiddleInitial"].ToString()
                                        : "null";
                                recordMap["Prescriber.LastName"] = thisPrescriberName["LastName"] != null
                                    ? thisPrescriberName["LastName"].ToString()
                                    : "null";
                            }
                            else
                            {
                                recordMap["Prescriber.FirstName"] = "null";
                                recordMap["Prescriber.MiddleInitial"] = "null";
                                recordMap["Prescriber.LastName"] = "null";
                            }

                            if (thisPrescriber["Address"] != null)
                            {
                                var thisPrescriberAddress =
                                    JsonConvert.DeserializeObject<Dictionary<string, object>>(
                                        thisPrescriber["Address"].ToString() ?? "{}");
                                recordMap["Prescriber.Street1"] = thisPrescriberAddress["Street1"] != null
                                    ? thisPrescriberAddress["Street1"].ToString()
                                    : "null";
                                recordMap["Prescriber.Street2"] = thisPrescriberAddress["Street2"] != null
                                    ? thisPrescriberAddress["Street2"].ToString()
                                    : "null";
                                recordMap["Prescriber.City"] = thisPrescriberAddress["City"] != null
                                    ? thisPrescriberAddress["City"].ToString()
                                    : "null";
                                recordMap["Prescriber.State"] = thisPrescriberAddress["State"] != null
                                    ? thisPrescriberAddress["State"].ToString()
                                    : "null";
                                recordMap["Prescriber.Zip"] = thisPrescriberAddress["Zip"] != null
                                    ? thisPrescriberAddress["Zip"].ToString()
                                    : "null";
                            }

                            else
                            {
                                recordMap["Prescriber.Street1"] = "null";
                                recordMap["Prescriber.Street2"] = "null";
                                recordMap["Prescriber.City"] = "null";
                                recordMap["Prescriber.State"] = "null";
                                recordMap["Prescriber.Zip"] = "null";
                            }
                        }

                        else
                        {
                            recordMap["Prescriber.Id"] = "null";
                            recordMap["Prescriber.Phone"] = "null";
                            recordMap["Prescriber.Fax"] = "null";
                            recordMap["Prescriber.NPI"] = "null";
                            recordMap["Prescriber.DEA"] = "null";
                            recordMap["Prescriber.CustomField1"] = "null";
                            recordMap["Prescriber.CustomField2"] = "null";
                            recordMap["Prescriber.CustomField3"] = "null";
                            recordMap["Prescriber.CustomField4"] = "null";
                            recordMap["Prescriber.FirstName"] = "null";
                            recordMap["Prescriber.MiddleInitial"] = "null";
                            recordMap["Prescriber.LastName"] = "null";
                            recordMap["Prescriber.Street1"] = "null";
                            recordMap["Prescriber.Street2"] = "null";
                            recordMap["Prescriber.City"] = "null";
                            recordMap["Prescriber.State"] = "null";
                            recordMap["Prescriber.Zip"] = "null";
                        }

                        if (Script["Fill"] != null)
                        {
                            var thisFill =
                                JsonConvert.DeserializeObject<Dictionary<string, object>>(
                                    Script["Fill"].ToString() ?? "{}");
                            recordMap["Fill.RefillNumber"] = thisFill["RefillNumber"] != null
                                ? thisFill["RefillNumber"].ToString()
                                : "0";
                            recordMap["Fill.DispenseDate"] = thisFill["DispenseDate"] != null
                                ? thisFill["DispenseDate"].ToString()
                                : "null";
                            recordMap["Fill.DispenseQuantity"] = thisFill["DispenseQuantity"] != null
                                ? thisFill["DispenseQuantity"].ToString()
                                : "0";
                            recordMap["Fill.DaysSupply"] = thisFill["DaysSupply"] != null
                                ? thisFill["DaysSupply"].ToString()
                                : "0";
                            recordMap["Fill.SIG"] =
                                thisFill["SIG"] != null ? thisFill["SIG"].ToString() : "null";
                            recordMap["Fill.DAW"] =
                                thisFill["DAW"] != null ? thisFill["DAW"].ToString() : "null";
                            recordMap["Fill.RphInitials"] = thisFill["RphInitials"] != null
                                ? thisFill["RphInitials"].ToString()
                                : "null";
                            recordMap["Fill.RphName"] = thisFill["RphName"] != null
                                ? thisFill["RphName"].ToString()
                                : "null";
                            recordMap["Fill.Status"] = thisFill["Status"] != null
                                ? thisFill["Status"].ToString()
                                : "null";
                            recordMap["Fill.StatusCode"] = thisFill["StatusCode"] != null
                                ? thisFill["StatusCode"].ToString()
                                : "null";
                            recordMap["Fill.PatientPay"] = thisFill["PatientPay"] != null
                                ? thisFill["PatientPay"].ToString()
                                : "0";
                            recordMap["Fill.ExpirationDate"] = thisFill["ExpirationDate"] != null
                                ? thisFill["ExpirationDate"].ToString()
                                : "null";
                            recordMap["Fill.LotNumber"] = thisFill["LotNumber"] != null
                                ? thisFill["LotNumber"].ToString()
                                : "null";
                            recordMap["Fill.WorfkflowLocation"] = thisFill["WorkflowLocation"] != null
                                ? thisFill["WorkflowLocation"].ToString()
                                : "null";
                            recordMap["Fill.Cost"] =
                                thisFill["Cost"] != null ? thisFill["Cost"].ToString() : "0";
                            recordMap["Fill.ACQ"] =
                                thisFill["ACQ"] != null ? thisFill["ACQ"].ToString() : "0";
                            recordMap["Fill.AWP"] =
                                thisFill["AWP"] != null ? thisFill["AWP"].ToString() : "0";
                            recordMap["Fill.UsualAndCustomary"] = thisFill["UsualAndCustomary"] != null
                                ? thisFill["UsualAndCustomary"].ToString()
                                : "0";
                            recordMap["Fill.Doses"] = thisFill["Doses"] != null
                                ? thisFill["Doses"].ToString()
                                : "0";

                            if (thisFill["Primary"] != null)
                            {
                                var thisFillPrimary =
                                    JsonConvert.DeserializeObject<Dictionary<string, object>>(
                                        thisFill["Primary"].ToString() ?? "{}");
                                recordMap["Fill.PrimaryId"] = thisFillPrimary["Id"] != null
                                    ? thisFillPrimary["Id"].ToString()
                                    : "null";
                                recordMap["Fill.PrimaryName"] = thisFillPrimary["Name"] != null
                                    ? thisFillPrimary["Name"].ToString()
                                    : "null";
                                recordMap["Fill.PrimaryBIN"] = thisFillPrimary["BIN"] != null
                                    ? thisFillPrimary["BIN"].ToString()
                                    : "null";
                                recordMap["Fill.PrimaryPCN"] = thisFillPrimary["PCN"] != null
                                    ? thisFillPrimary["PCN"].ToString()
                                    : "null";
                                recordMap["Fill.PrimaryInsurancePay"] =
                                    thisFillPrimary["InsurancePay"] != null
                                        ? thisFillPrimary["InsurancePay"].ToString()
                                        : "0";
                                recordMap["Fill.PrimaryPatientPay"] = thisFillPrimary["PatientPay"] != null
                                    ? thisFillPrimary["PatientPay"].ToString()
                                    : "0";
                            }
                            else
                            {
                                recordMap["Fill.PrimaryId"] = "null";
                                recordMap["Fill.PrimaryName"] = "null";
                                recordMap["Fill.PrimaryBIN"] = "null";
                                recordMap["Fill.PrimaryPCN"] = "null";
                                recordMap["Fill.PrimaryInsurancePay"] = "0";
                                recordMap["Fill.PrimaryPatientPay"] = "0";
                            }

                            if (thisFill["Secondary"] != null)
                            {
                                var thisFillSecondary =
                                    JsonConvert.DeserializeObject<Dictionary<string, object>>(
                                        thisFill["Secondary"].ToString() ?? "{}");
                                recordMap["Fill.SecondaryId"] = thisFillSecondary["Id"] != null
                                    ? thisFillSecondary["Id"].ToString()
                                    : "null";
                                recordMap["Fill.SecondaryName"] = thisFillSecondary["Name"] != null
                                    ? thisFillSecondary["Name"].ToString()
                                    : "null";
                                recordMap["Fill.SecondaryBIN"] = thisFillSecondary["BIN"] != null
                                    ? thisFillSecondary["BIN"].ToString()
                                    : "null";
                                recordMap["Fill.SecondaryPCN"] = thisFillSecondary["PCN"] != null
                                    ? thisFillSecondary["PCN"].ToString()
                                    : "null";
                                recordMap["Fill.SeocondaryInsurancePay"] =
                                    thisFillSecondary["InsurancePay"] != null
                                        ? thisFillSecondary["PatientPay"].ToString()
                                        : "0";
                                recordMap["Fill.SecondaryPatientPay"] =
                                    thisFillSecondary["PatientPay"] != null
                                        ? thisFillSecondary["PatientPay"].ToString()
                                        : "0";
                            }

                            else
                            {
                                recordMap["Fill.SecondaryId"] = "null";
                                recordMap["Fill.SecondaryName"] = "null";
                                recordMap["Fill.SecondaryBIN"] = "null";
                                recordMap["Fill.SecondaryPCN"] = "null";
                                recordMap["Fill.SeocondaryInsurancePay"] = "0";
                                recordMap["Fill.SecondaryPatientPay"] = "0";
                            }
                        }
                        else
                        {
                            recordMap["Fill.RefillNumber"] = "0";
                            recordMap["Fill.DispenseDate"] = "null";
                            recordMap["Fill.DispenseQuantity"] = "0";
                            recordMap["Fill.DaysSupply"] = "0";
                            recordMap["Fill.SIG"] = "null";
                            recordMap["Fill.DAW"] = "null";
                            recordMap["Fill.RphInitials"] = "null";
                            recordMap["Fill.RphName"] = "null";
                            recordMap["Fill.Status"] = "null";
                            recordMap["Fill.StatusCode"] = "null";
                            recordMap["Fill.PatientPay"] = "null";
                            recordMap["Fill.ExpirationDate"] = "null";
                            recordMap["Fill.LotNumber"] = "null";
                            recordMap["Fill.WorfkflowLocation"] = "null";
                            recordMap["Fill.Cost"] = "0";
                            recordMap["Fill.ACQ"] = "0";
                            recordMap["Fill.AWP"] = "0";
                            recordMap["Fill.UsualAndCustomary"] = "0";
                            recordMap["Fill.Doses"] = "0";
                            recordMap["Fill.SecondaryId"] = "null";
                            recordMap["Fill.SecondaryName"] = "null";
                            recordMap["Fill.SecondaryBIN"] = "null";
                            recordMap["Fill.SecondaryPCN"] = "null";
                            recordMap["Fill.SeocondaryInsurancePay"] = "0";
                            recordMap["Fill.SecondaryPatientPay"] = "0";
                            recordMap["Fill.PrimaryId"] = "null";
                            recordMap["Fill.PrimaryName"] = "null";
                            recordMap["Fill.PrimaryBIN"] = "null";
                            recordMap["Fill.PrimaryPCN"] = "null";
                            recordMap["Fill.PrimaryInsurancePay"] = "0";
                            recordMap["Fill.PrimaryPatientPay"] = "0";
                        }

                        yield return new Record
                        {
                            Action = Record.Types.Action.Upsert,
                            DataJson = JsonConvert.SerializeObject(recordMap)
                        };
                    }

                    var page = objectResponseWrapper.Page;
                    var recordCount = objectResponseWrapper.RecordCount;
                    var pageSize = objectResponseWrapper.PageSize;

                    hasMore = page * pageSize <= recordCount;
                    pageNumber++;
                } while (hasMore);
            }
        }

        public static readonly Dictionary<string, Endpoint> PrescriptionEndpoints = new Dictionary<string, Endpoint>
        {
            {
                "AllPrescriptions", new PrescriptionsEndpoint
                {
                    Id = "AllPrescriptions",
                    ShouldGetStaticSchema = true,
                    Name = "AllPrescriptions",
                    BasePath = "https://api.libertysoftware.com",
                    AllPath = $"/prescriptions",
                    PropertiesPath = "/crm/v3/properties/prescriptions",
                    SupportedActions = new List<EndpointActions>
                    {
                        EndpointActions.Get
                    },
                    PropertyKeys = new List<string>
                    {
                    }
                }
            },
        };
    }
}