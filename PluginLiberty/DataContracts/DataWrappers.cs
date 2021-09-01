using System.Collections.Generic;
using Newtonsoft.Json;

namespace PluginLiberty.DataContracts
{
    public class PrescriptionResponseWrapper
    {
        [JsonProperty("Scripts")] 
        public List<Dictionary<string, object>> Scripts { get; set; }
        [JsonProperty("results")]
        public List<ObjectResponse> Results { get; set; }
        
        [JsonProperty("paging")]
        public PagingResponse Paging { get; set; }
    }

    public class PatientResponseWrapper
    {
        [JsonProperty("")]
        public List<Dictionary<string, object>> Patient { get; set; }

    }

    public class AccountsReceivableWrapper
    {
        [JsonProperty("OwnerPatientId")]
        public string? OwnerPatientId { get; set; }

        [JsonProperty("AccountNumber")]
        public int AccountNumber { get; set; }

        [JsonProperty("ChargeCode")]
        public string? ChargeCode { get; set; }

        [JsonProperty("CreditLimit")]
        public float CreditLimit { get; set; }

        [JsonProperty("LastPaymentAmount")]
        public float LastPaymentAmount { get; set; }

        [JsonProperty("LastPaymentDate")]
        public string? LastPaymentDate { get; set; }

        [JsonProperty("PreviousBalance")]
        public float PreviousBalance { get; set; }

        [JsonProperty("AmountDue")]
        public float AmountDue { get; set; }

        [JsonProperty("Over30Days")]
        public float Over30Days { get; set; }

        [JsonProperty("Over60Days")]
        public float Over60Days { get; set; }

        [JsonProperty("Over90Days")]
        public float Over90Days { get; set; }

        [JsonProperty("Over120Days")]
        public float Over120Days { get; set; }

        [JsonProperty("CurrentDebits")]
        public float CurrentDebits { get; set; }

        [JsonProperty("CurrentCredits")]
        public float CurrentCredits { get; set; }

        [JsonProperty("TotalBalance")]
        public float TotalBalance { get; set; }
    }

    public class ObjectResponse
    {
        [JsonProperty("ScriptNumber")]
        public int ScriptNumber { get; set; }
        
        [JsonProperty("properties")]
        public Dictionary<string, object> Properties { get; set; }
    }


    public class PropertyResponseWrapper
    {
        [JsonProperty("Scripts")]
        public List<Dictionary<string, object>> Scripts {get; set;}
        
        [JsonProperty("paging")]
        public PagingResponse Paging { get; set; }
    }

    public class PropertyResponse
    {
        [JsonProperty("name")]
        public string Id { get; set; }
        
        [JsonProperty("label")]
        public string Name { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("hasUniqueValue")]
        public bool IsKey { get; set; }
        
        [JsonProperty("calculated")]
        public bool Calculated { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("modificationMetadata")]
        public ModificationMetaData ModificationMetaData { get; set; }
    }

    public class PagingResponse
    {
        [JsonProperty("next")]
        public NextResponse Next { get; set; }
    }

    public class NextResponse
    {
        [JsonProperty("after")]
        public string After { get; set; }
        
        [JsonProperty("link")]
        public string Link { get; set; }
    }
}