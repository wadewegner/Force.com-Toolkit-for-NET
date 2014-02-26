using System;

namespace Salesforce.Chatter.Models
{
    [Obsolete("Use Reference class instead of this.", false)]
    public class LikedItem
    {
        public string id { get; set; }
        public string url { get; set; }
    }
}