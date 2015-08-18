using System.Collections.Generic;
using System.Xml.Serialization;

namespace Salesforce.Force.Bulk.Models
{
    public interface ISObjectList<T> : IList<T>, IXmlSerializable
    {
    }
}
