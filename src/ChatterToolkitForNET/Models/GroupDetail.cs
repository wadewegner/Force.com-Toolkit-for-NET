using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class GroupDetail
    {

        [JsonProperty(PropertyName = "canHaveChatterGuests")]
        public bool CanHaveChatterGuests { get; set; }

        [JsonProperty(PropertyName = "community")]
        public Reference Community { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "emailToChatterAddress")]
        public string EmailToChatterAddress { get; set; }

        [JsonProperty(PropertyName = "fileCount")]
        public int? FileCount { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "information")]
        public GroupInformation Information { get; set; }

        [JsonProperty(PropertyName = "lastFeedItemPostDate")]
        public string LastFeedItemPostDate { get; set; }

        [JsonProperty(PropertyName = "memberCount")]
        public int MemberCount { get; set; }

        [JsonProperty(PropertyName = "motif")]
        public Motif Motif { get; set; }

        [JsonProperty(PropertyName = "myRole")]
        public string MyRole { get; set; }

        [JsonProperty(PropertyName = "mySubscription")]
        public Reference MySubscription { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public UserSummary Owner { get; set; }

        [JsonProperty(PropertyName = "photo")]
        public Photo Photo { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "visibility")]
        public string Visibility { get; set; }
    }
}