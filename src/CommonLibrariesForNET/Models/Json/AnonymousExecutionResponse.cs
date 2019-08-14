using Newtonsoft.Json;

namespace Salesforce.Common.Models.Json
{
    public class AnonymousExecutionResponse
    {
        [JsonProperty(PropertyName = "line")]
        public int Line { get; set; }
        [JsonProperty(PropertyName = "column")]
        public int Column { get; set; }
        [JsonProperty(PropertyName = "compiled")]
        public bool Compiled { get; set; }
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }
        [JsonProperty(PropertyName = "exceptionStackTrace")]
        public string ExceptionStackTrace { get; set; }
        [JsonProperty(PropertyName = "exceptionMessage")]
        public string ExceptionMessage { get; set; }
        [JsonProperty(PropertyName = "compileProblem")]
        public string CompileProblem { get; set; }

    }
}

