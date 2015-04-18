using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class Comment
    {
        [JsonProperty(PropertyName = "attachment")]
        public object Attachment { get; set; }

        [JsonProperty(PropertyName = "body")]
        public FeedBody Body { get; set; }

        [JsonProperty(PropertyName = "clientInfo")]
        public ClientInfo ClientInfo { get; set; }

        [JsonProperty(PropertyName = "createdDate")]
        public string CreatedDate { get; set; }

        [JsonProperty(PropertyName = "feedItem")]
        public Reference FeedItem { get; set; }

        [JsonProperty(PropertyName = "id")]
         public string Id { get; set; }

        [JsonProperty(PropertyName = "isDeleteRestricted")]
        public bool IsDeleteRestricted { get; set; }

        [JsonProperty(PropertyName = "likes")]
        public LikePage Likes { get; set; }

        [JsonProperty(PropertyName = "likesMessage")]
        public LikeMessageBody LikesMessage { get; set; }

        [JsonProperty(PropertyName = "myLike")]
        public Reference MyLike { get; set; }

        [JsonProperty(PropertyName = "parent")]
        public Reference Parent { get; set; }

        [JsonProperty(PropertyName = "relativeCreatedDate")]
        public string RelativeCreatedDate { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "user")]
        public UserSummary User { get; set; }
    }
}