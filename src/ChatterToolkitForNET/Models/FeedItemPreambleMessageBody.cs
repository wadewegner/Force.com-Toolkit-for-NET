using System.Collections.Generic;
using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class FeedItemPreambleMessageBody
    {
        [JsonProperty(PropertyName = "messageSegments")]
        public List<MessageSegment> MessageSegments { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
    }
}