using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class FeedItem
    {
        [JsonProperty(PropertyName = "actor")]
        public Actor Actor { get; set; }

        [JsonProperty(PropertyName = "attachment")]
        public object Attachment { get; set; }

        [JsonProperty(PropertyName = "body")]
        public FeedBody Body { get; set; }

        [JsonProperty(PropertyName = "canShare")]
        public bool CanShare { get; set; }
        
        [JsonProperty(PropertyName = "clientInfo")]
        public ClientInfo ClientInfo { get; set; }

        [JsonProperty(PropertyName = "comments")]
        public CommentPage Comments { get; set; }

        [JsonProperty(PropertyName = "createdDate")]
        public string CreatedDate { get; set; }

        [JsonProperty(PropertyName = "event")]
        public bool Event { get; set; }
        
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "isBookmarkedByCurrentUser")]
        public bool IsBookmarkedByCurrentUser { get; set; }

        [JsonProperty(PropertyName = "isDeleteRestricted")]
        public bool IsDeleteRestricted { get; set; }

        [JsonProperty(PropertyName = "isLikedByCurrentUser")]
        public bool IsLikedByCurrentUser { get; set; }

        [JsonProperty(PropertyName = "likes")]
        public LikePage Likes { get; set; }

        [JsonProperty(PropertyName = "likesMessage")]
        public LikeMessageBody LikesMessage { get; set; }

        [JsonProperty(PropertyName = "myLike")]
        public Reference MyLike { get; set; }

        [JsonProperty(PropertyName = "modifiedDate")]
        public string ModifiedDate { get; set; }

        [JsonProperty(PropertyName = "moderationFlags")]
        public ModerationFlags ModerationFlags { get; set; }

        [JsonProperty(PropertyName = "originalFeedItem")]
        public Reference OriginalFeedItem { get; set; }

        [JsonProperty(PropertyName = "originalFeedItemActor")]
        public UserSummary OriginalFeedItemActor { get; set; }

        [JsonProperty(PropertyName = "parent")]
        public UserSummary Parent { get; set; }

        [JsonProperty(PropertyName = "photoUrl")]
        public string PhotoUrl { get; set; }

        [JsonProperty(PropertyName = "preamble")]
        public FeedItemPreambleMessageBody Preamble { get; set; }

        [JsonProperty(PropertyName = "relativeCreatedDate")]
        public string RelativeCreatedDate { get; set; }

        [JsonProperty(PropertyName = "topics")]
        public FeedItemTopics Topics { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "visibility")]
        public string Visibility { get; set; }
    }
}