using System.Collections.Generic;
using Newtonsoft.Json;

namespace PluginLiberty.DataContracts
{
    public class PrescriptionResponseWrapper
    {
        [JsonProperty("Scripts")]
        public List<Dictionary<string, object>> Scripts { get; set; }
        
        [JsonProperty("RecordCount")]
        public long RecordCount { get; set; }

        [JsonProperty("Page")]
        public long Page { get; set; }
        
        [JsonProperty("PageSize")]
        public long PageSize { get; set; }
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

    public class ClaimsWrapper
    {
        [JsonProperty("ScriptNumber")]
        public string? ScriptNumber { get; set; }

        [JsonProperty("RefillNumber")]
        public string? RefillNumber { get; set; }

        [JsonProperty("DateSubmitted")]
        public string? DateSubmitted { get; set; }

        [JsonProperty("Coverage")]
        public string? Coverage { get; set; }

        [JsonProperty("Type")]
        public string? Type { get; set; }

        [JsonProperty("Status")]
        public string? Status { get; set; }

        [JsonProperty("Message")]
        public string? Message { get; set; }

        [JsonProperty("AuthNumber")]
        public string? AuthNumber { get; set; }

        [JsonProperty("PayorId")]
        public string? PayorId { get; set; }

        [JsonProperty("BIN")]
        public string? BIN { get; set; }

        [JsonProperty("PCN")]
        public string? PCN { get; set; }

        [JsonProperty("DrugId")]
        public string? DrugId { get; set; }

        [JsonProperty("BasisOfCost")]
        public string? BasisOfCost { get; set; }

        [JsonProperty("BasisOfReimbursement")]
        public string? BasisOfReimbursement { get; set; }

        [JsonProperty("RequestedACQ")]
        public string? RequestedACQ { get; set; }

        [JsonProperty("RequestedCost")]
        public string? RequestedCost { get; set; }

        [JsonProperty("RequestedServiceFee")]
        public string? RequestedServiceFee { get; set; }

        [JsonProperty("RequestedDispensingFee")]
        public string? RequestedDispensingFee { get; set; }

        [JsonProperty("RequestedCopay")]
        public string? RequestedCopay { get; set; }

        [JsonProperty("RequestedTax")]
        public string? RequestedTax { get; set; }

        [JsonProperty("RequestedIncentive")]
        public string? RequestedIncentive { get; set; }

        [JsonProperty("RequestedUC")]
        public string? RequestedUC { get; set; }

        [JsonProperty("RequestedTotal")]
        public string? RequestedTotal { get; set; }

        [JsonProperty("RepliedCost")]
        public string? RepliedCost { get; set; }

        [JsonProperty("RepliedServiceFee")]
        public string? RepliedServiceFee { get; set; }

        [JsonProperty("RepliedDispensingFee")]
        public string? RepliedDispensingFee { get; set; }

        [JsonProperty("RepliedCopay")]
        public string? RepliedCopay { get; set; }

        [JsonProperty("RepliedTax")]
        public string? RepliedTax { get; set; }

        [JsonProperty("RepliedIncentive")]
        public string? RepliedIncentive { get; set; }

        [JsonProperty("RepliedTotal")]
        public string? RepliedTotal { get; set; }

        [JsonProperty("OtherPayorAmt")]
        public string? OtherPayorAmt { get; set; }
    
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
        public List<Dictionary<string, object>> Scripts { get; set; }

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