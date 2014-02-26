using System.Collections.Generic;

namespace Salesforce.Chatter.Models
{
    public class GroupPage
    {
        public string currentPageUrl { get; set; }
        public List<GroupDetail> groups { get; set; }
        public string nextPageUrl { get; set; }
        public string previousPageUrl { get; set; }
    }
}