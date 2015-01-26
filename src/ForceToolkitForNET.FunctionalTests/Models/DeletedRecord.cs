using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.Force.FunctionalTests.Models
{
    public class DeletedRecord
    {
        public string id { get; set; }
        public string deletedDate { get; set; }
    }

    public class DeletedRecordRootObject
    {
        public List<DeletedRecord> deletedRecords { get; set; }
        public string earliestDateAvailable { get; set; }
        public string latestDateCovered { get; set; }
    }
}
