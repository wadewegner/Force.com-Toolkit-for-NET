using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.Common.Models
{
    public class AttributeInfo
    {
        [JsonProperty(PropertyName = "type")]
        public string TypeName { get; set; }

        [JsonProperty(PropertyName = "referenceId")]
        public string ReferenceId { get; set; }
    }
}
