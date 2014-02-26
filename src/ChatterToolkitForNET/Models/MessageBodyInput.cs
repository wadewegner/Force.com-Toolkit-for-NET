using System.Collections.Generic;

namespace Salesforce.Chatter.Models
{
    public class MessageBodyInput
    {
       public List<MessageSegmentInput> messageSegments { get; set; }
    }
}