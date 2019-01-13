using System.Xml.Serialization;

namespace Salesforce.Common.Models.Xml
{
    [XmlRoot(Namespace = "http://www.force.com/2009/06/asyncapi/dataload",
     ElementName = "jobInfo",
     IsNullable = false)]
    public class JobInfo
    {
        [XmlElement(ElementName = "operation")]
        public string Operation { get; set; }

        [XmlElement(ElementName = "object")]
        public string Object { get; set; }

        [XmlElement(ElementName = "externalIdFieldName")]
        public string ExternalIdFieldName { get; set; }

        [XmlIgnore]
        public bool ExternalIdFieldNameSpecified => !string.IsNullOrEmpty(ExternalIdFieldName);

        [XmlElement(ElementName = "contentType")]
        public string ContentType { get; set; }

    }
}
