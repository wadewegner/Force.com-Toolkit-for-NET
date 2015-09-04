using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class ObjectFeedItemInput : FeedItemInput
    {
        [JsonProperty(PropertyName = "feedElementType")]
        readonly public string FeedItem = "FeedItem";

        [JsonProperty(PropertyName = "subjectId")]
        public string ObjectId { get; set; }

        [JsonProperty(PropertyName = "capabilities")]
        public Capabilities Capabilities { get; set; }
    }
}