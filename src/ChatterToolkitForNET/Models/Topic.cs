using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class Topic
    {
        [JsonProperty(PropertyName = "createdDate")]
        public string CreatedDate { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "talkingAbout")]
        public int? TalkingAbout { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }
}
