using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Salesforce.Common.Models
{
    public class DescribeGlobalResult<T>
    {
        public string encoding { get; set; }
        public int maxBatchSize { get; set; }
        public List<T> sobjects { get; set; }
    }
}