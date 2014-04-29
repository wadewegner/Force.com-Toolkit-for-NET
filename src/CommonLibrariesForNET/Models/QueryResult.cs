using System.Collections.Generic;

namespace Salesforce.Common.Models
{
    public class QueryResult<T>
    {
        public string nextRecordsUrl { get; set; }
        public int totalSize { get; set; }
        public string done { get; set; }
        public List<T> records { get; set; }
    }
}