namespace Salesforce.Chatter.Models
{
    public class UserSummary
    {
        public string companyName { get; set; }
        public string firstName { get; set; }
        public string id { get; set; }
        public bool isActive { get; set; }
        public bool isInThisCommunity { get; set; }
        public string lastName { get; set; }
        public Motif motif { get; set; }
        public Reference mySubscription { get; set; }
        public string name { get; set; }
        public Photo photo { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string userType { get; set; }
    }
}