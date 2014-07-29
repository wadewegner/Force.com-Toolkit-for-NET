using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.Force.UnitTests.Models
{
    public class ObjectDescribeMetadata
    {
        public ObjectFieldMetadata[] Fields { get; set; }
        public string Label { get; set; }
        public string LabelPlural { get; set; }
        public string Name { get; set; }

    }
}
