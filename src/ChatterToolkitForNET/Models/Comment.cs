namespace Salesforce.Chatter.Models
{
    public class Comment
    {
        public object attachment { get; set; }
        public FeedBody body { get; set; }
        public ClientInfo clientInfo { get; set; }
        public string createdDate { get; set; }
        public Reference feedItem { get; set; }
         public string id { get; set; }
        public bool isDeleteRestricted { get; set; }
        public LikePage likes { get; set; }
        public LikeMessageBody likesMessage { get; set; }
        public Reference myLike { get; set; }
        public Reference parent { get; set; }
        public string relativeCreatedDate { get; set; }
        public string type { get; set; }

        public string url { get; set; }
        public UserSummary user { get; set; }
    }
}