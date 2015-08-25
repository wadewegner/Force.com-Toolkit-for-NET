using Newtonsoft.Json;

namespace Salesforce.Common.Models
{
    public class Attributes
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }
}