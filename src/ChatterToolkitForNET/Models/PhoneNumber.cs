using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class PhoneNumber
    {
        [JsonProperty(PropertyName = "phoneNumber")]
        public string Number { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }
}