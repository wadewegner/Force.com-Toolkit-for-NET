using System.Collections.Generic;
using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class FeedItemPage
    {
        [JsonProperty(PropertyName = "currentPageUrl")]
        public string CurrentPageUrl { get; set; }

        [JsonProperty(PropertyName = "isModifiedToken")]
        public string IsModifiedToken { get; set; }

        [JsonProperty(PropertyName = "isModifiedUrl")]
        public string IsModifiedUrl { get; set; }

        [JsonProperty(PropertyName = "items")]
        public List<FeedItem> Items { get; set; }

        [JsonProperty(PropertyName = "nextPageUrl")]
        public string NextPageUrl { get; set; }
    }
}