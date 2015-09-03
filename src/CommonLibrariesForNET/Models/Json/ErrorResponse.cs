using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Salesforce.Common.Models.Json
{
    [XmlRoot(Namespace = "http://www.force.com/2009/06/asyncapi/dataload",
     ElementName = "error",
     IsNullable = false)]
    public class ErrorResponse
    {
        [XmlElement(ElementName = "exceptionCode")]
        [JsonProperty(PropertyName = "message")]
        public string Message;

        [XmlElement(ElementName = "exceptionMessage")]
        [JsonProperty(PropertyName = "errorCode")]
        public string ErrorCode;
    }
}
