using System.Collections.Generic;

namespace Salesforce.Chatter.Models
{
    public class TopicCollection
    {
        public string currentPageUrl { get; set; }
        public string nextPageUrl { get; set; }
        public List<Topic> topics { get; set; }
    }
}