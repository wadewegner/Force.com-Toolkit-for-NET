using System.Collections.Generic;

namespace Salesforce.Chatter.Models
{
    public class FeedItemTopics
    {
        public List<object> topics { get; set; }
        public bool canAssignTopics { get; set; }
    }
}