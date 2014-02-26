using System.Collections.Generic;

namespace Salesforce.Chatter.Models
{
    public class UserPage
    {
        public string currentPageUrl { get; set; }
        public string nextPageUrl { get; set; }
        public string previousPageUrl { get; set; }
        public List<UserDetail> users { get; set; }
    }
}