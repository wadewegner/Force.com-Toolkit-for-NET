using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class Address
    {
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        [JsonProperty(PropertyName = "formattedAddress")]
        public string FormattedAddress { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "street")]
        public string Street { get; set; }

        [JsonProperty(PropertyName = "zip")]
        public string Zip { get; set; }
    }
}