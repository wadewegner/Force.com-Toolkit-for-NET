using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class FollowingCounts
    {
        [JsonProperty(PropertyName = "people")]
        public int People { get; set; }

        [JsonProperty(PropertyName = "records")]
        public int Records { get; set; }

        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }
    }
}