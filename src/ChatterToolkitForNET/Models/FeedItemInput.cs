using Newtonsoft.Json;
using System;

namespace Salesforce.Chatter.Models
{
    public class FeedItemInput
    {
        [JsonProperty(PropertyName = "attachment")]
        public Attachment Attachment { get; set; }

        [JsonProperty(PropertyName = "body")]
        public MessageBodyInput Body { get; set; }

        [JsonProperty(PropertyName = "subjectId")]
        public String SubjectId { get; set; }

        [JsonProperty(PropertyName = "feedElementType")]
        public String FeedElementType { get; set; }
    }
}