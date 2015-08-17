using System.Collections.Generic;
using System.Xml.Serialization;

namespace Salesforce.Force.Bulk.Models
{
    [XmlRoot(ElementName = "sObjects",
             Namespace = "http://www.force.com/2009/06/asyncapi/dataload")]
    public class SObjectListGeneric<T> : List<T>, ISObjectList
    {
    }
}
