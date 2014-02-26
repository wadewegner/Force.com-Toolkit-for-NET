using System.Collections.Generic;

namespace Salesforce.Chatter.Models
{
    public class FeedItemPage
    {
        public string currentPageUrl { get; set; }
        public string isModifiedToken { get; set; }
        public string isModifiedUrl { get; set; }
        public List<FeedItem> items { get; set; }
        public string nextPageUrl { get; set; }
    }
}