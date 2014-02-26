namespace Salesforce.Chatter.Models
{
    public class GroupDetail
    {
 
        public bool canHaveChatterGuests { get; set; }
        public Reference community { get; set; }
        public string description { get; set; }
        public string emailToChatterAddress { get; set; }
        public int fileCount { get; set; }
        public string id { get; set; }
        public GroupInformation information { get; set; }
        public string lastFeedItemPostDate { get; set; }
        public int memberCount { get; set; }
        public Motif motif { get; set; }
        public string myRole { get; set; }
        public Reference mySubscription { get; set; }
        public string name { get; set; }
        public UserSummary owner { get; set; }
        public Photo photo { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string visibility { get; set; }
    }
}