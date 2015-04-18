using System.Collections.Generic;
using Newtonsoft.Json;

namespace Salesforce.Force.FunctionalTests.Models
{
    public class UpdatedRecordRootObject
    {
        [JsonProperty(PropertyName = "ids")]
        public List<string> Ids { get; set; }

        [JsonProperty(PropertyName = "latestDateCovered")]
        public string LatestDateCovered { get; set; }
    }
}
