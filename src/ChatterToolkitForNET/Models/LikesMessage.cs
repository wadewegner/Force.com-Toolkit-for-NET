using System.Collections.Generic;

namespace Salesforce.Chatter.Models
{
    public class LikeMessageBody
    {
        public List<MessageSegment> messageSegments { get; set; }
        public string text { get; set; }
    }
}