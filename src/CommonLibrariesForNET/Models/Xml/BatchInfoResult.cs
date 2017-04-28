using System;
using System.Xml.Serialization;

namespace Salesforce.Common.Models.Xml
{
     [XmlRoot(Namespace = "http://www.force.com/2009/06/asyncapi/dataload",
     ElementName = "batchInfo",
     IsNullable = false)]
    public class BatchInfoResult
    {
         [XmlElement(ElementName = "id")]
         public string Id { get; set; }

         [XmlElement(ElementName = "jobId")]
         public string JobId { get; set; }

         [XmlElement(ElementName = "state")]
         public string State { get; set; }

         [XmlElement(ElementName = "createdDate")]
         public DateTime CreatedDate { get; set; }

         [XmlElement(ElementName = "systemModstamp")]
         public DateTime SystemModstamp { get; set; }

         [XmlElement(ElementName = "numberRecordsProcessed")]
         public int NumberRecordsProcessed { get; set; }

         protected bool Equals(BatchInfoResult other)
         {
             return string.Equals(Id, other.Id);
         }

         public override bool Equals(object obj)
         {
             if (ReferenceEquals(null, obj)) return false;
             if (ReferenceEquals(this, obj)) return true;
             return obj.GetType() == GetType() && Equals((BatchInfoResult) obj);
         }

         public override int GetHashCode()
         {
             return (Id != null ? Id.GetHashCode() : 0);
         }
    }
}
