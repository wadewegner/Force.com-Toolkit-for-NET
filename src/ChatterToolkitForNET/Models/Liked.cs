using System;
namespace Salesforce.Chatter.Models
{
    [Obsolete("Use Like class instead of this.", false)]
    public class Liked
    {
        public string id { get; set; }
        public string url { get; set; }
        public UserSummary user { get; set; }
        public LikedItem likedItem { get; set; }
    }
}