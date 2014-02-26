using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Salesforce.Chatter.Models
{
    public class Topic
    {
        public string createdDate { get; set; }
        public string description { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public int? talkingAbout { get; set; }
        public string url { get; set; }
    }
}
