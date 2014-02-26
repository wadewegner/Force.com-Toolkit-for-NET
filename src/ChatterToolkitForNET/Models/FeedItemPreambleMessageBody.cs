using System.Collections.Generic;

namespace Salesforce.Chatter.Models
{
    public class FeedItemPreambleMessageBody
    {
        public List<MessageSegment> messageSegments { get; set; }
        public string text { get; set; }
    }
}