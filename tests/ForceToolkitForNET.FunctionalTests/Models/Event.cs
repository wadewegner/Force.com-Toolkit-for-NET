using System;

namespace Salesforce.Force.FunctionalTests.Models
{
    public class Event
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        public string WhatId { get; set; }
        public DateTime ActivityDate { get; set; }
        public DateTime ActivityDateTime { get; set; }
        public int DurationInMinutes { get; set; }

    }
}
