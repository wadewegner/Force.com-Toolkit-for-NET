using System;
namespace Salesforce.Chatter.Models
{
    [Obsolete("Use FeedItem class instead of this.", false)]
    public class Item
    {
        public Actor actor { get; set; } // The entity that created the item. This can be a RecordSummary, a UserSummary, or an UnauthenticatedUser (i.e. Chatter customer).
        public object attachment { get; set; } // Depends on the type value of the attachment.
        public FeedBody body { get; set; }
        public bool canShare { get; set; }
        public ClientInfo clientInfo { get; set; }
        public CommentPage comments { get; set; }
        public string createdDate { get; set; }
        public bool @event { get; set; }
        public string id { get; set; }
        public bool isBookmarkedByCurrentUser { get; set; }
        public bool isDeleteRestricted { get; set; }
        public LikeMessageBody ByCurrentUser { get; set; }
        public LikePage likes { get; set; }
        public LikeMessageBody likesMessage { get; set; }
        public object moderationFlags { get; set; }
        public string modifiedDate { get; set; }
        public Reference myLike { get; set; }
        public Reference originalFeedItem { get; set; }
        public UserSummary originalFeedItemActor { get; set; }
        public UserSummary parent { get; set; }
        public string photoUrl { get; set; }
        public FeedItemPreambleMessageBody preamble { get; set; }
        public string relativeCreatedDate { get; set; }
        public FeedItemTopics topics { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string visibility { get; set; }
    }
}