using Newtonsoft.Json;

namespace Salesforce.Common.Models.Json
{

    public class SuccessResponseBatchSave
    {
        public bool hasErrors { get; set; }
        public dynamic results { get; set; }
    }

    public class SuccessResponseBatchSaveResult
    {
        public int statusCode { get; set; }
        public SuccessResponseBatchSaveResult1 result { get; set; }
    }

    public class SuccessResponseBatchSaveResult1
    {
        public SuccessResponseBatchSaveResult1Attributes attributes { get; set; }
        public string Name { get; set; }
        public string BillingPostalCode { get; set; }
        public string Id { get; set; }
    }

    public class SuccessResponseBatchSaveResult1Attributes
    {
        public string type { get; set; }
        public string url { get; set; }
    }


}

