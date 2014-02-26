namespace Salesforce.Chatter.Models
{
    public class MessageSegment
    {
        public Motif motif { get; set; }
        public object record { get; set; }
        public Reference reference { get; set; }
        public string text { get; set; }
        public string type { get; set; }
        public string url { get; set; }
    }
}