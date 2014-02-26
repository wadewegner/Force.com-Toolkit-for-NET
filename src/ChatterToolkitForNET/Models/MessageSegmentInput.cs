namespace Salesforce.Chatter.Models
{
    public class MessageSegmentInput
    {
        public string id { get; set; } // Mention
        public string tag { get; set; } // Hash Tag
        public string text { get; set; } // Text
        public string type { get; set; }
        public string url { get; set; } // Link
    }
}