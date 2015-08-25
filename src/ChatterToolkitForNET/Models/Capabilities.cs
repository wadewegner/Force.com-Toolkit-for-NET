using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class Capabilities
    {
        [JsonProperty(PropertyName = "content")]
        public Content Content { get; set; }
    }
}
