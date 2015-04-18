using System.Collections.Generic;
using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class FeedItemTopics
    {
        [JsonProperty(PropertyName = "topics")]
        public List<object> Topics { get; set; }

        [JsonProperty(PropertyName = "canAssignTopics")]
        public bool CanAssignTopics { get; set; }
    }
}