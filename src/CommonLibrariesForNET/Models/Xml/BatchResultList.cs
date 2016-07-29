using System.Collections.Generic;
using System.Xml.Serialization;

namespace Salesforce.Common.Models.Xml
{
    [XmlRoot(ElementName = "results",
        Namespace = "http://www.force.com/2009/06/asyncapi/dataload")]
    public class BatchResultList
    {
        public BatchResultList()
        {
            Items = new List<BatchResult>();
        }

        [XmlElement("result")]
        public List<BatchResult> Items;
    }
}
