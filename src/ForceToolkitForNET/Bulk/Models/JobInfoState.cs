using System.Xml.Serialization;

namespace Salesforce.Force.Bulk.Models
{
    [XmlRoot(Namespace = "http://www.force.com/2009/06/asyncapi/dataload",
     ElementName = "jobInfo",
     IsNullable = false)]
    public class JobInfoState
    {
        [XmlElement(ElementName = "state")]
        public string State { get; set; }
    }
}
