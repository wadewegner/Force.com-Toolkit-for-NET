using Newtonsoft.Json;

namespace Salesforce.Common.Models
{
    public class SearchResult
    {
        [JsonProperty(PropertyName = "attributes")]
        public Attributes Attributes { get; set; }

        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }
    }
}
