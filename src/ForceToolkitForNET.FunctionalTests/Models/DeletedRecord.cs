using System.Collections.Generic;
using Newtonsoft.Json;

namespace Salesforce.Force.FunctionalTests.Models
{
    public class DeletedRecord
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "deletedDate")]
        public string DeletedDate { get; set; }
    }

    public class DeletedRecordRootObject
    {
        [JsonProperty(PropertyName = "deletedRecords")]
        public List<DeletedRecord> DeletedRecords { get; set; }

        [JsonProperty(PropertyName = "earliestDateAvailable")]
        public string EarliestDateAvailable { get; set; }

        [JsonProperty(PropertyName = "latestDateCovered")]
        public string LatestDateCovered { get; set; }
    }
}
