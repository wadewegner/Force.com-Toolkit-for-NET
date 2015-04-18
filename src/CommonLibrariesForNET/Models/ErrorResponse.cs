using Newtonsoft.Json;

namespace Salesforce.Common.Models
{
    public class ErrorResponse
    {
        [JsonProperty(PropertyName = "message")]
        public string Message;

        [JsonProperty(PropertyName = "errorCode")]
        public string ErrorCode;
    }
}
