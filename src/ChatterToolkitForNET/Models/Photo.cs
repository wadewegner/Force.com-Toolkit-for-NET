using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class Photo
    {
        [JsonProperty(PropertyName = "fullEmailPhotoUrl")]
        public string FullEmailPhotoUrl { get; set; }

        [JsonProperty(PropertyName = "largePhotoUrl")]
        public string LargePhotoUrl { get; set; }

        [JsonProperty(PropertyName = "photoVersionId")]
        public string PhotoVersionId { get; set; }

        [JsonProperty(PropertyName = "smallPhotoUrl")]
        public string SmallPhotoUrl { get; set; }

        [JsonProperty(PropertyName = "standardEmailPhotoUrl")]
        public string StandardEmailPhotoUrl { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }
}