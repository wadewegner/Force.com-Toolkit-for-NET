using System.Collections.Generic;

namespace Salesforce.Chatter.Models
{
    public class Attachment
    {
        public string attachmentType { get; set; }
        public string url { get; set; }
        public string urlName { get; set; }
        public List<string> pollChoices { get; set; }
        public string developerName { get; set; }
        public string namespacePrefix { get; set; }
        public string parameters { get; set; }
        public string height { get; set; }
        public string title { get; set; }
        public string contentDocumentId { get; set; }
        public string description { get; set; }
    }
}