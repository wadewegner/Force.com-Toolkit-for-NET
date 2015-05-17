using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class FeedItemInput
    {
        [JsonProperty(PropertyName = "attachment")]
        public Attachment Attachment { get; set; }

        [JsonProperty(PropertyName = "body")]
        public MessageBodyInput Body { get; set; }


    }

    public class ObjectFeedItemInput : FeedItemInput
    {
        

        [JsonProperty(PropertyName = "feedElementType")]
        readonly public string FeedItem = "FeedItem";

        [JsonProperty(PropertyName = "subjectId")]
        public string ObjectID { get; set; }

        [JsonProperty(PropertyName = "capabilities")]
        public Capabilities Capabilities { get; set; }

    }
}