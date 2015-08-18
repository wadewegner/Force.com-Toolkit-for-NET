using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Salesforce.Force.Bulk.Models
{
    public sealed class SObject : Dictionary<string,object>, IXmlSerializable
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
            writer.WriteRaw("<sObject>");
            foreach (var entry in this)
            {
                if (entry.Value.GetType() != typeof (SObject))
                {
                    writer.WriteRaw(string.Format("<{0}>{1}</{0}>", entry.Key, entry.Value));
                }
                else
                {
                    writer.WriteRaw(string.Format("<{0}>", entry.Key));
                    ((SObject) entry.Value).WriteXml(writer);
                    writer.WriteRaw(string.Format("</{0}>", entry.Key));
                }
            }
            writer.WriteRaw("</sObject>");
        }

    }
}
