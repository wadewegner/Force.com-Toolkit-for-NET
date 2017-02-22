using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Salesforce.Common.Models.Xml
{
    public sealed class SObject : Dictionary<string,object>, IXmlSerializable
    {

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteRaw("<sObject>");
            foreach (var entry in this)
            {
                var value = entry.Value as IXmlSerializable;
                if (value != null)
                {
                    writer.WriteRaw(string.Format("<{0}>", entry.Key));
                    value.WriteXml(writer);
                    writer.WriteRaw(string.Format("</{0}>", entry.Key));
                }
                else
                {
                    writer.WriteRaw(string.Format("<{0}>{1}</{0}>", entry.Key, entry.Value));
                }
            }
            writer.WriteRaw("</sObject>");
        }

    }
}
