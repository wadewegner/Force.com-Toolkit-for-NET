using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.Common.Models
{
    public class ErrorResult
    {
        [JsonProperty(PropertyName = "statusCode")]
        public string StatusCode { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "fields")]
        public List<string> Fields { get; set; }
    }
}
