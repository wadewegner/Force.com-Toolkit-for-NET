using Newtonsoft.Json;

namespace Salesforce.Common.Models
{
    public class SuccessResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string Id;

        [JsonProperty(PropertyName = "success")]
        public string Success;

        [JsonProperty(PropertyName = "errors")]
        public object Errors;
    }
}

