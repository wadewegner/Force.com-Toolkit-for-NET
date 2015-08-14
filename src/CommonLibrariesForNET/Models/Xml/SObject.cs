using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Salesforce.Common.Models.Xml
{
    public sealed class SObject : Dictionary<string,string>, IXmlSerializable
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
                writer.WriteRaw(string.Format("<{0}>{1}</{0}>", entry.Key, entry.Value));
            }
        }

    }
}
