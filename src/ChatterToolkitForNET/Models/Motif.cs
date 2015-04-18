using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class Motif
    {
        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; }

        [JsonProperty(PropertyName = "largeIconUrl")]
        public string LargeIconUrl { get; set; }

        [JsonProperty(PropertyName = "mediumIconUrl")]
        public string MediumIconUrl { get; set; }

        [JsonProperty(PropertyName = "smallIconUrl")]
        public string SmallIconUrl { get; set; }
    }
}