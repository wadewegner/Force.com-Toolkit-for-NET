using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.Common.Models
{
    public class SaveResult
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty(PropertyName = "errors")]
        public List<Error> Errors { get; set; }
    }

}
