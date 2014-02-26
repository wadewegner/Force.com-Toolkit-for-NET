using System.Collections.Generic;

namespace Salesforce.Chatter.Models
{
    public class FeedBody
    {
       public List<MessageSegment> messageSegments { get; set; }
       public string text { get; set; }
    }
}