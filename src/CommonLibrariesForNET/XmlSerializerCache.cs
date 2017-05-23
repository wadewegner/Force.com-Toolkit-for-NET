using System.Collections.Generic;
using System.Xml.Serialization;

namespace Salesforce.Common
{

    public class XmlSerializerCache
    {
        private static XmlSerializerCache _instance;

        private XmlSerializerCache()
        {
            XmlSerializerDictionary = new Dictionary<string, XmlSerializer>();
        }

        public static XmlSerializerCache Instance => _instance ?? (_instance = new XmlSerializerCache());
        public Dictionary<string, XmlSerializer> XmlSerializerDictionary { get; set; }
    }
}
