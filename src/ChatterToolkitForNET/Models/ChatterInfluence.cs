using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class ChatterInfluence
    {
        [JsonProperty(PropertyName = "percentile")]
        public string Percentile { get; set; }

        [JsonProperty(PropertyName = "rank")]
        public int Rank { get; set; }
    }
}