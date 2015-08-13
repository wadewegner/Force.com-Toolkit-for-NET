using System;
using System.Xml.Serialization;

namespace Salesforce.Common.Models.Xml
{
    [XmlRoot(Namespace = "http://www.force.com/2009/06/asyncapi/dataload",
     ElementName = "jobInfo",
     IsNullable = false)]
    public class JobInfoResult : JobInfo
    {
        [XmlElement(ElementName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "createdById")]
        public string CreatedById { get; set; }

        [XmlElement(ElementName = "createdDate")]
        public DateTime CreatedDate { get; set; }

        [XmlElement(ElementName = "systemModstamp")]
        public DateTime SystemModstamp { get; set; }

        [XmlElement(ElementName = "state")]
        public string State { get; set; }

        [XmlElement(ElementName = "concurrencyMode")]
        public string ConcurrencyMode { get; set; }

        [XmlElement(ElementName = "numberBatchesQueued")]
        public int NumberBatchesQueued { get; set; }

        [XmlElement(ElementName = "numberBatchesInProgress")]
        public int NumberBatchesInProgress { get; set; }

        [XmlElement(ElementName = "numberBatchesCompleted")]
        public int NumberBatchesCompleted { get; set; }

        [XmlElement(ElementName = "numberBatchesFailed")]
        public int NumberBatchesFailed { get; set; }

        [XmlElement(ElementName = "numberBatchesTotal")]
        public int NumberBatchesTotal { get; set; }

        [XmlElement(ElementName = "numberRecordsProcessed")]
        public int NumberRecordsProcessed { get; set; }

        [XmlElement(ElementName = "numberRetries")]
        public int NumberRetries { get; set; }

        [XmlElement(ElementName = "apiVersion")]
        public float ApiVersion { get; set; }
    }
}
