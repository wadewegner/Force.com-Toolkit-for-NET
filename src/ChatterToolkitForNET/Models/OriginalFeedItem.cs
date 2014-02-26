using System;

namespace Salesforce.Chatter.Models
{
    [Obsolete("Use Reference class instead of this.", false)]
    public class OriginalFeedItem
    {
        public string id { get; set; }
        public string url { get; set; }
    }
}