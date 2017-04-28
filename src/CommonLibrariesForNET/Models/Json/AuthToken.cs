using Newtonsoft.Json;

namespace Salesforce.Common.Models.Json
{
    public class AuthToken
    {
        [JsonProperty(PropertyName = "id")]
        public string Id;

        [JsonProperty(PropertyName = "issued_at")]
        public string IssuedAt;

        [JsonProperty(PropertyName = "instance_url")]
        public string InstanceUrl;

        [JsonProperty(PropertyName = "signature")]
        public string Signature;

        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken;

        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken;
    }
}
