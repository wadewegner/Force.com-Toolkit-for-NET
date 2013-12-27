using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForceSDKforNET.Models
{
    public class Urls
    {
        public string sobject { get; set; }
        public string describe { get; set; }
        public string rowTemplate { get; set; }
        public string quickActions { get; set; }
        public string layouts { get; set; }
        public string compactLayouts { get; set; }
        public string passwordUtilities { get; set; }
    }

    public class RootObject
    {
        public string encoding { get; set; }
        public int maxBatchSize { get; set; }
        public List<SObject> sObjects { get; set; }
    }
}
