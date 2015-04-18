using System.Collections.Generic;
using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class UserDetail
    {
        [JsonProperty(PropertyName = "aboutMe")]
        public string aboutMe { get; set; }

        [JsonProperty(PropertyName = "address")]
        public Address address { get; set; }

        [JsonProperty(PropertyName = "chatterActivity")]
        public ChatterActivity chatterActivity { get; set; }

        [JsonProperty(PropertyName = "chatterInfluence")]
        public ChatterInfluence chatterInfluence { get; set; }

        [JsonProperty(PropertyName = "companyName")]
        public string companyName { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string email { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string firstName { get; set; }

        [JsonProperty(PropertyName = "followersCount")]
        public int followersCount { get; set; }

        [JsonProperty(PropertyName = "followingCounts")]
        public FollowingCounts followingCounts { get; set; }

        [JsonProperty(PropertyName = "groupCount")]
        public int groupCount { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }

        [JsonProperty(PropertyName = "isActive")]
        public bool isActive { get; set; }

        [JsonProperty(PropertyName = "isInThisCommunity")]
        public bool isInThisCommunity { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string lastName { get; set; }

        [JsonProperty(PropertyName = "managerId")]
        public string managerId { get; set; }

        [JsonProperty(PropertyName = "managerName")]
        public string managerName { get; set; }

        [JsonProperty(PropertyName = "motif")]
        public Motif motif { get; set; }

        [JsonProperty(PropertyName = "mySubscription")]
        public Reference mySubscription { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }

        [JsonProperty(PropertyName = "phoneNumbers")]
        public List<PhoneNumber> phoneNumbers { get; set; }

        [JsonProperty(PropertyName = "photo")]
        public Photo photo { get; set; }

        [JsonProperty(PropertyName = "thanksReceived")]
        public int? thanksReceived { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string title { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string type { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string url { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string username { get; set; }

        [JsonProperty(PropertyName = "userType")]
        public string userType { get; set; }
    }
}