using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Salesforce.Common.Models
{
    public class QueryResult<T>
    {
        public int totalSize { get; set; }
        public string done { get; set; }
        public List<T> records { get; set; }
    }

}
