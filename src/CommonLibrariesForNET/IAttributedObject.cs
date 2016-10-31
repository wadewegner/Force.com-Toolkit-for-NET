using Newtonsoft.Json;
using Salesforce.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.Common
{
    /// <summary>
    /// Interface enforcing implementation of Attributes Property for multiple record updates
    /// </summary>
    public interface IAttributedObject
    {
        [JsonProperty(PropertyName = "attributes")]
        ObjectAttributes Attributes { get; set; }

    }
}
