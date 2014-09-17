using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salesforce.Common;
using Salesforce.Common.Attributes;

namespace Salesforce.Force.FunctionalTests.Models
{
    public class Contact
    {
        [Updateable(false), Createable(false)]
        public string Id { get; set; }

        [Updateable(false), Createable(false)]
        public bool IsDeleted { get; set; }

        public string MasterRecordId { get; set; }

        [Updateable(false), Createable(false)]
        public string AccountId { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Salutation { get; set; }

        [Updateable(false), Createable(false)]
        public string Name { get; set; }

        public string OtherStreet { get; set; }

        public string OtherCity { get; set; }

        public string OtherState { get; set; }

        public string OtherPostalCode { get; set; }

        public string OtherCountry { get; set; }

        [Updateable(false), Createable(false)]
        public double? OtherLatitude { get; set; }

        [Updateable(false), Createable(false)]
        public double? OtherLongitude { get; set; }

        public object OtherAddress { get; set; }

        public string MailingStreet { get; set; }

        public string MailingCity { get; set; }

        public string MailingState { get; set; }

        public string MailingPostalCode { get; set; }

        public string MailingCountry { get; set; }

        [Updateable(false), Createable(false)]
        public double? MailingLatitude { get; set; }

        [Updateable(false), Createable(false)]
        public double? MailingLongitude { get; set; }

        [Updateable(false), Createable(false)]
        public object MailingAddress { get; set; }

        public string ReportsToId { get; set; }

        public string Title { get; set; }

        public string Department { get; set; }

        public string AssistantName { get; set; }

        public string LeadSource { get; set; }

        public DateTimeOffset? Birthdate { get; set; }

        public string Description { get; set; }

        public string OwnerId { get; set; }

        [Updateable(false), Createable(false)]
        public DateTimeOffset CreatedDate { get; set; }

        [Updateable(false), Createable(false)]
        public string CreatedById { get; set; }

        [Updateable(false), Createable(false)]
        public DateTimeOffset LastModifiedDate { get; set; }

        [Updateable(false), Createable(false)]
        public string LastModifiedById { get; set; }

        [Updateable(false), Createable(false)]
        public DateTimeOffset SystemModstamp { get; set; }

        [Updateable(false), Createable(false)]
        public DateTimeOffset? LastActivityDate { get; set; }

        [Updateable(false), Createable(false)]
        public DateTimeOffset? LastCURequestDate { get; set; }

        [Updateable(false), Createable(false)]
        public DateTimeOffset? LastCUUpdateDate { get; set; }

        [Updateable(false), Createable(false)]
        public DateTimeOffset? LastViewedDate { get; set; }

        [Updateable(false), Createable(false)]
        public DateTimeOffset? LastReferencedDate { get; set; }

        [Updateable(false), Createable(false)]
        public string EmailBouncedReason { get; set; }

        [Updateable(false), Createable(false)]
        public DateTimeOffset? EmailBouncedDate { get; set; }

        [Updateable(false), Createable(false)]
        public bool IsEmailBounced { get; set; }

        [Updateable(false), Createable(false)]
        public string Jigsaw { get; set; }

        [Updateable(false), Createable(false)]
        public string JigsawContactId { get; set; }

        public string Level__c { get; set; }

        public string Languages__c { get; set; }
    }
}
