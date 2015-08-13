using System.Xml.Serialization;

namespace Salesforce.Common.Models.Xml
{
    [XmlRoot(Namespace = "http://www.force.com/2009/06/asyncapi/dataload",
     ElementName = "error",
     IsNullable = false)]
    public class ErrorResponse
    {
        [XmlElement(ElementName = "exceptionCode")]
        public string ErrorCode { get; set; }

        [XmlElement(ElementName = "exceptionMessage")]
        public string Message { get; set; }
    }
}
