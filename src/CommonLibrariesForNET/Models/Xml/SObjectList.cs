using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Salesforce.Common.Models.Xml
{
    [XmlRoot(Namespace = "http://www.force.com/2009/06/asyncapi/dataload", ElementName = "sObjects")]
    public sealed class SObjectList<T> : List<T>, ISObjectList<T>
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
            foreach (var entry in this)
            {
                var value = entry as IXmlSerializable;
                if (value != null)
                {
                    value.WriteXml(writer);
                }
                else
                {
                    var xmlSerializer = new XmlSerializer(typeof(T), new XmlRootAttribute("sObject"));
                    var ns = new XmlSerializerNamespaces();
                    ns.Add(string.Empty, string.Empty);
                    var settings = new XmlWriterSettings {OmitXmlDeclaration = true};
                    var stringBuilder = new StringBuilder();
                    using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
                    {
                        xmlSerializer.Serialize(xmlWriter, entry, ns);
                    }
                    writer.WriteRaw(stringBuilder.ToString());
                }
            }
        }
    }
}
