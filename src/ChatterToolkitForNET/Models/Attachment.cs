using System.Collections.Generic;
using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class Attachment
    {
        [JsonProperty(PropertyName = "attachmentType")]
        public string AttachmentType { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "urlName")]
        public string UrlName { get; set; }

        [JsonProperty(PropertyName = "pollChoices")]
        public List<string> PollChoices { get; set; }

        [JsonProperty(PropertyName = "developerName")]
        public string DeveloperName { get; set; }

        [JsonProperty(PropertyName = "namespacePrefix")]
        public string NamespacePrefix { get; set; }

        [JsonProperty(PropertyName = "parameters")]
        public string Parameters { get; set; }

        [JsonProperty(PropertyName = "height")]
        public string Height { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "contentDocumentId")]
        public string ContentDocumentId { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}