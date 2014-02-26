using System.Collections.Generic;

namespace Salesforce.Chatter.Models
{
    public class CommentPage
    {
        public List<Comment> comments { get; set; }
        public string currentPageUrl { get; set; }
        public string nextPageUrl { get; set; }
        public int total { get; set; }
    }
}