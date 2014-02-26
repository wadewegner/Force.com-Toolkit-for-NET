using System.Collections.Generic;

namespace Salesforce.Chatter.Models
{
    public class UserDetail
    {
        public string aboutMe { get; set; }
        public Address address { get; set; }
        public ChatterActivity chatterActivity { get; set; }
        public ChatterInfluence chatterInfluence { get; set; }
        public string companyName { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public int followersCount { get; set; }
        public FollowingCounts followingCounts { get; set; }
        public int groupCount { get; set; }
        public string id { get; set; }
        public bool isActive { get; set; }
        public bool isInThisCommunity { get; set; }
        public string lastName { get; set; }
        public string managerId { get; set; }
        public string managerName { get; set; }
        public Motif motif { get; set; }
        public Reference mySubscription { get; set; }
        public string name { get; set; }
        public List<PhoneNumber> phoneNumbers { get; set; }
        public Photo photo { get; set; }
        public int? thanksReceived { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string username { get; set; }
        public string userType { get; set; }
    }
}