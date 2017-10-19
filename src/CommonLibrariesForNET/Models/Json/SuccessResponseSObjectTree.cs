using Newtonsoft.Json;

namespace Salesforce.Common.Models.Json
{
    public class SuccessResponseSObjectTree
    {
        //[JsonProperty(PropertyName = "hasErrors")]
        public bool hasErrors { get; set; }
        public SuccessResponseObjectTreeResult[] results { get; set; }
    }
    public class SuccessResponseObjectTreeResult
    {
        public string referenceId { get; set; }
        public string id { get; set; }
    }
}

