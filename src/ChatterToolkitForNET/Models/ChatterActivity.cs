using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    // me

    public class ChatterActivity
    {
        public int commentCount { get; set; }
        public int commentReceivedCount { get; set; }
        public int likeReceivedCount { get; set; }
        public int postCount { get; set; }
    }

}
