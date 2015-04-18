using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class ChatterActivity
    {
        [JsonProperty(PropertyName = "commentCount")]
        public int CommentCount { get; set; }

        [JsonProperty(PropertyName = "commentReceivedCount")]
        public int CommentReceivedCount { get; set; }

        [JsonProperty(PropertyName = "likeReceivedCount")]
        public int LikeReceivedCount { get; set; }

        [JsonProperty(PropertyName = "postCount")]
        public int PostCount { get; set; }
    }

}
