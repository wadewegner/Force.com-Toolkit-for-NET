using System.Collections.Generic;
using System.Xml.Serialization;

namespace Salesforce.Common.Models.Xml
{
    public interface ISObjectList<T> : IList<T>, IXmlSerializable
    {
    }
}
