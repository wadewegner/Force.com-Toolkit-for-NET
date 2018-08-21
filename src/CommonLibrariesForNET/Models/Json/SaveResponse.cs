using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.Common.Models.Json
{
    public class SaveResponse
    {
        [JsonProperty(PropertyName = "hasErrors")]
        public bool HasErrors { get; set; }

        [JsonProperty(PropertyName = "results")]
        public List<SaveResult> Results { get; set; }
    }
}
