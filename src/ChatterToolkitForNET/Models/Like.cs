namespace Salesforce.Chatter.Models
{
    public class Like
    {
        public string id { get; set; }
        public Reference likedItem { get; set; }
        public string url { get; set; }
        public UserSummary user { get; set; }

    }
}