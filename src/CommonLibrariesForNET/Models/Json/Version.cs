using System;
using System.Collections.Generic;
using System.Text;

namespace Salesforce.Common.Models.Json
{
    public class Version : IComparable<Version>
    {
        public string label { get; set; }
        public string url { get; set; }
        public string version { get; set; }

        public int CompareTo(Version other)
        {
            if (other == null)
                return -1;
            return string.Compare(version, other.version);
        }
    }
}
