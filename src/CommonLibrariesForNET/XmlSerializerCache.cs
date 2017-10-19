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

        public static XmlSerializerCache GetInstance()
        {
            if (_instance == null)
                _instance = new XmlSerializerCache();
            return _instance;
        }

        public Dictionary<string, XmlSerializer> XmlSerializerDictionary { get; set; }
    }
}
