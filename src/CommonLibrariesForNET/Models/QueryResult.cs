using System.Collections.Generic;

namespace Salesforce.Common.Models
{
    public class QueryResult<T>
    {
        /// <summary>
        ///     Used to perform subsequent data requests if a query will return more than 2000 records
        ///     If empty, then all data has been returned for the query
        /// </summary>
        public string nextRecordsUrl { get; set; }

        public int totalSize { get; set; }
        public string done { get; set; }
        public List<T> records { get; set; }
    }
}