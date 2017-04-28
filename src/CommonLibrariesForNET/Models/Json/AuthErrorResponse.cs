using Newtonsoft.Json;

namespace Salesforce.Common.Models.Json
{
    public class AuthErrorResponse
    {
        [JsonProperty(PropertyName = "error_description")]
        public string ErrorDescription;

        [JsonProperty(PropertyName = "error")]
        public string Error;
    }
}