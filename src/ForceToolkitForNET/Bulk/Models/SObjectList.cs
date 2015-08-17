using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Salesforce.Force.Bulk.Models
{
    [XmlRoot(ElementName="sObjects",
             Namespace = "http://www.force.com/2009/06/asyncapi/dataload")]
    public sealed class SObjectList : List<SObject>, IXmlSerializable, ISObjectList
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new System.NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var entry in this)
            {
                writer.WriteRaw("<sObject>");
                entry.WriteXml(writer);
                writer.WriteRaw("</sObject>");
            }
        }
    }
}
