using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System;
using NUnit.Framework;
using System.Threading.Tasks;
using Salesforce.Common;
using Salesforce.Common.Models.Xml;
using Salesforce.Force.Tests.Models;

namespace Salesforce.Force.Tests
{
    [TestFixture]
    public class BulkForceClientFunctionalTests
    {
        private static string _consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
        private static string _consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
        private static string _username = ConfigurationManager.AppSettings["Username"];
        private static string _password = ConfigurationManager.AppSettings["Password"];
        private static string _organizationId = ConfigurationManager.AppSettings["OrganizationId"];
        private static bool _testUpsert;

        private AuthenticationClient _auth;
        private ForceClient _client;

        [OneTimeSetUp]
        public void Init()
        {
            bool.TryParse(ConfigurationManager.AppSettings["TestUpsert"], out _testUpsert);
            if (string.IsNullOrEmpty(_consumerKey) && string.IsNullOrEmpty(_consumerSecret) && string.IsNullOrEmpty(_username) && string.IsNullOrEmpty(_password) && string.IsNullOrEmpty(_organizationId))
            {
                _consumerKey = Environment.GetEnvironmentVariable("ConsumerKey");
                _consumerSecret = Environment.GetEnvironmentVariable("ConsumerSecret");
                _username = Environment.GetEnvironmentVariable("Username");
                _password = Environment.GetEnvironmentVariable("Password");
                _organizationId = Environment.GetEnvironmentVariable("OrganizationId");
                bool.TryParse(Environment.GetEnvironmentVariable("TestUpsert"), out _testUpsert);
            }

            // Use TLS 1.2 (instead of defaulting to 1.0)
            const int SecurityProtocolTypeTls11 = 768;
            const int SecurityProtocolTypeTls12 = 3072;
            ServicePointManager.SecurityProtocol |= (SecurityProtocolType)(SecurityProtocolTypeTls12 | SecurityProtocolTypeTls11); 

            _auth = new AuthenticationClient();
            _auth.UsernamePasswordAsync(_consumerKey, _consumerSecret, _username, _password).Wait();
            _client = new ForceClient(_auth.InstanceUrl, _auth.AccessToken, _auth.ApiVersion);
        }

        [Test]
        public async Task FullRunThrough()
        {
            // Make a strongly typed Account list
            var stAccountsBatch = new SObjectList<Account>
            {
                new Account {Name = "TestStAccount1"},
                new Account {Name = "TestStAccount2"},
                new Account {Name = "TestStAccount3"}
            };

            // insert the accounts
            var results1 = await _client.RunJobAndPollAsync("Account", BulkConstants.OperationType.Insert,
                    new List<SObjectList<Account>> { stAccountsBatch });
            // (one SObjectList<T> per batch, the example above uses one batch)

            Assert.IsTrue(results1 != null, "[results1] empty result object");
            Assert.AreEqual(results1.Count, 1, "[results1] wrong number of results");
            Assert.AreEqual(results1[0].Items.Count, 3, "[results1] wrong number of result records");
            Assert.IsTrue(results1[0].Items[0].Created);
            Assert.IsTrue(results1[0].Items[0].Success);
            Assert.IsTrue(results1[0].Items[1].Created);
            Assert.IsTrue(results1[0].Items[1].Success);
            Assert.IsTrue(results1[0].Items[2].Created);
            Assert.IsTrue(results1[0].Items[2].Success);


            // Make a dynamic typed Account list
            var dtAccountsBatch = new SObjectList<SObject>
            {
                new SObject{{"Name", "TestDtAccount1"}},
                new SObject{{"Name", "TestDtAccount2"}},
                new SObject{{"Name", "TestDtAccount3"}}
            };

            // insert the accounts
            var results2 = await _client.RunJobAndPollAsync("Account", BulkConstants.OperationType.Insert,
                    new List<SObjectList<SObject>> { dtAccountsBatch });

            Assert.IsTrue(results2 != null, "[results2] empty result object");
            Assert.AreEqual(results2.Count, 1, "[results2] wrong number of results");
            Assert.AreEqual(results2[0].Items.Count, 3, "[results2] wrong number of result records");
            Assert.IsTrue(results2[0].Items[0].Created);
            Assert.IsTrue(results2[0].Items[0].Success);
            Assert.IsTrue(results2[0].Items[1].Created);
            Assert.IsTrue(results2[0].Items[1].Success);
            Assert.IsTrue(results2[0].Items[2].Created);
            Assert.IsTrue(results2[0].Items[2].Success);

            // get the id of the first account created in the first batch
            var id = results2[0].Items[0].Id;
            dtAccountsBatch = new SObjectList<SObject>
            {
                new SObject
                {
                    {"Id", id},
                    {"Name", "TestDtAccount1Renamed"}
                }
            };

            // update the first accounts name (dont really need bulk for this, just an example)
            var results3 = await _client.RunJobAndPollAsync("Account", BulkConstants.OperationType.Update,
                    new List<SObjectList<SObject>> { dtAccountsBatch });

            Assert.IsTrue(results3 != null);
            Assert.AreEqual(results3.Count, 1);
            Assert.AreEqual(results3[0].Items.Count, 1);
            Assert.AreEqual(results3[0].Items[0].Id, id);
            Assert.IsFalse(results3[0].Items[0].Created);
            Assert.IsTrue(results3[0].Items[0].Success);

            // create an Id list for the original strongly typed accounts created
            var idBatch = new SObjectList<SObject>();
            idBatch.AddRange(results1[0].Items.Select(result => new SObject { { "Id", result.Id } }));

            // delete all the strongly typed accounts
            var results4 = await _client.RunJobAndPollAsync("Account", BulkConstants.OperationType.Delete,
                    new List<SObjectList<SObject>> { idBatch });



            Assert.IsTrue(results4 != null, "[results4] empty result object");
            Assert.AreEqual(results4.Count, 1, "[results4] wrong number of results");
            Assert.AreEqual(results4[0].Items.Count, 3, "[results4] wrong number of result records");
            Assert.IsFalse(results4[0].Items[0].Created);
            Assert.IsTrue(results4[0].Items[0].Success);
        }
    }

        [Test]
        public async Task UpsertTests()
        {
            // Requires a new field on Contact "Unique_Email__c" with External Id set.
            if (_testUpsert)
            {
                var dtContactsBatch1 = new SObjectList<SObject>
                {
                    new SObject{{ "FirstName", "TestDtContact1"}, { "LastName", "TestDtContact1" }, { "MailingCity", "London" }, { "Email", "email1@example.com" }, {"Unique_Email__c", "email1@example.com"}},
                    new SObject{{ "FirstName", "TestDtContact2"}, { "LastName", "TestDtContact2" }, { "MailingCity", "London" }, { "Email", "email2@example.com" }, {"Unique_Email__c", "email2@example.com" }}
                };

                var resultsUpsert1 = await _client.RunJobAndPollAsync("Contact", "Unique_Email__c",
                    BulkConstants.OperationType.Upsert, new List<SObjectList<SObject>> { dtContactsBatch1 });

                Assert.IsTrue(resultsUpsert1 != null);
                Assert.AreEqual(resultsUpsert1.Count, 1);
                Assert.AreEqual(resultsUpsert1[0].Items.Count, 2);
                Assert.IsTrue(resultsUpsert1[0].Items[0].Created);
                Assert.IsTrue(resultsUpsert1[0].Items[0].Success);
                Assert.IsTrue(resultsUpsert1[0].Items[1].Created);
                Assert.IsTrue(resultsUpsert1[0].Items[1].Success);

                var dtContactsBatch2 = new SObjectList<SObject>
                {
                    new SObject{{ "FirstName", "TestDtContact2"}, { "LastName", "TestDtContact2" }, { "MailingCity", "York" }, { "Email", "email2@example.com" }, {"Unique_Email__c", "email2@example.com" }},
                    new SObject{{ "FirstName", "TestDtContact3"}, { "LastName", "TestDtContact3" }, { "MailingCity", "York" }, { "Email", "email3@example.com" }, {"Unique_Email__c", "email3@example.com" }}
                };

                var resultsUpsert2 = await _client.RunJobAndPollAsync("Contact", "Unique_Email__c",
                    BulkConstants.OperationType.Upsert, new List<SObjectList<SObject>> { dtContactsBatch2 });

                Assert.IsTrue(resultsUpsert2 != null);
                Assert.AreEqual(resultsUpsert2.Count, 1);
                Assert.AreEqual(resultsUpsert2[0].Items.Count, 2);
                Assert.IsFalse(resultsUpsert2[0].Items[0].Created);
                Assert.IsTrue(resultsUpsert2[0].Items[0].Success);
                Assert.IsTrue(resultsUpsert2[0].Items[1].Created);
                Assert.IsTrue(resultsUpsert2[0].Items[1].Success);

                // create an Id list for the original strongly typed accounts created
                var idBatch = new SObjectList<SObject>();
                idBatch.AddRange(resultsUpsert1[0].Items.Select(result => new SObject { { "Id", result.Id } }));
                idBatch.Add(new SObject { {"Id", resultsUpsert2[0].Items[1].Id}});

                var resultsDelete = await _client.RunJobAndPollAsync("Contact", BulkConstants.OperationType.Delete,
                    new List<SObjectList<SObject>> { idBatch });

                Assert.IsTrue(resultsDelete != null, "[results4] empty result object");
                Assert.AreEqual(resultsDelete.Count, 1, "[results4] wrong number of results");
                Assert.AreEqual(resultsDelete[0].Items.Count, 3, "[results4] wrong number of result records");
            }
            else
            {
                Assert.Inconclusive("Upsert Tests Skipped.");
            }

        }
    }
}
