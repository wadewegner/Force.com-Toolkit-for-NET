using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.Common.Models
{
    public class CreateRequest
    {
        [JsonProperty(PropertyName = "records")]
        public List<IAttributedObject> records { get; set; }
    }
}
