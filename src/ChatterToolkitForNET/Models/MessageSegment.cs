using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class MessageSegment
    {
        [JsonProperty(PropertyName = "motif")]
        public Motif Motif { get; set; }

        [JsonProperty(PropertyName = "record")]
        public object Record { get; set; }

        [JsonProperty(PropertyName = "reference")]
        public Reference Reference { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }
}