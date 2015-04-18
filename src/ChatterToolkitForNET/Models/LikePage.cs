using System.Collections.Generic;
using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class LikePage
    {
        [JsonProperty(PropertyName = "currentPageUrl")]
        public string CurrentPageUrl { get; set; }

        [JsonProperty(PropertyName = "likes")]
        public List<Like> Likes { get; set; }

        [JsonProperty(PropertyName = "nextPageUrl")]
        public string NextPageUrl { get; set; }

        [JsonProperty(PropertyName = "previousPageUrl")]
        public string PreviousPageUrl { get; set; }

        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }
    }
}