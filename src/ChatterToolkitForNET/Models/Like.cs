using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class Like
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "likedItem")]
        public Reference LikedItem { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "user")]
        public UserSummary User { get; set; }

    }
}