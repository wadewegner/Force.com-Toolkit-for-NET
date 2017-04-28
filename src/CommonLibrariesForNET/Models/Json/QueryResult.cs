using System.Collections.Generic;
using Newtonsoft.Json;

namespace Salesforce.Common.Models.Json
{
    public class QueryResult<T>
    {
        [JsonProperty(PropertyName = "nextRecordsUrl")]
        public string NextRecordsUrl { get; set; }

        [JsonProperty(PropertyName = "totalSize")]
        public int TotalSize { get; set; }

        [JsonProperty(PropertyName = "done")]
        public bool Done { get; set; }

        [JsonProperty(PropertyName = "records")]
        public List<T> Records { get; set; }
    }
}