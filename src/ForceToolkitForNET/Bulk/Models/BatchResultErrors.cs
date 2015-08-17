using System.Collections.Generic;
using System.Xml.Serialization;

namespace Salesforce.Force.Bulk.Models
{
    [XmlType(TypeName = "errors")]
    public class BatchResultErrors
    {
        [XmlElement(ElementName = "fields")]
        public List<string> Fields { get; set; }

        [XmlElement(ElementName = "message")]
        public string Message { get; set; }

        [XmlElement(ElementName = "statusCode")]
        public string StatusCode { get; set; }
    }
}
