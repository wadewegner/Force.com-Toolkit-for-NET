using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class Content
    {
        [JsonProperty(PropertyName = "description")]
        public string  Description { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
    }
}
