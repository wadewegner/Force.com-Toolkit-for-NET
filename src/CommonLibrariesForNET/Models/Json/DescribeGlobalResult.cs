using System.Collections.Generic;
using Newtonsoft.Json;

namespace Salesforce.Common.Models.Json
{
    public class DescribeGlobalResult<T>
    {
        [JsonProperty(PropertyName = "encoding")]
        public string Encoding { get; set; }

        [JsonProperty(PropertyName = "maxBatchSize")]
        public int MaxBatchSize { get; set; }

        [JsonProperty(PropertyName = "sobjects")]
        public List<T> SObjects { get; set; }
    }
}