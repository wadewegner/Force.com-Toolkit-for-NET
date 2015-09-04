using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using Salesforce.Common;
using Salesforce.Common.Models.Xml;
using Salesforce.Force.FunctionalTests.Models;

namespace Salesforce.Force.FunctionalTests
{
    [TestFixture]
    public class BulkForceClientTests
    {
        private static readonly string SecurityToken = ConfigurationManager.AppSettings["SecurityToken"];
        private static readonly string ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
        private static readonly string ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
        private static readonly string Username = ConfigurationManager.AppSettings["Username"];
        private static readonly string Password = ConfigurationManager.AppSettings["Password"] + SecurityToken;

        private AuthenticationClient _auth;
        private ForceClient _client;

        [TestFixtureSetUp]
        public void Init()
        {
            _auth = new AuthenticationClient();
            _auth.UsernamePasswordAsync(ConsumerKey, ConsumerSecret, Username, Password).Wait();
            _client = new ForceClient(_auth.InstanceUrl, _auth.AccessToken, _auth.ApiVersion);
        }

        [Test]
        public async void FullRunThrough()
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
            Assert.AreEqual(results1[0].Count, 3, "[results1] wrong number of result records");
            Assert.IsTrue(results1[0][0].Created);
            Assert.IsTrue(results1[0][0].Success);
            Assert.IsTrue(results1[0][1].Created);
            Assert.IsTrue(results1[0][1].Success);
            Assert.IsTrue(results1[0][2].Created);
            Assert.IsTrue(results1[0][2].Success);


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
            Assert.AreEqual(results2[0].Count, 3, "[results2] wrong number of result records");
            Assert.IsTrue(results2[0][0].Created);
            Assert.IsTrue(results2[0][0].Success);
            Assert.IsTrue(results2[0][1].Created);
            Assert.IsTrue(results2[0][1].Success);
            Assert.IsTrue(results2[0][2].Created);
            Assert.IsTrue(results2[0][2].Success);

            // get the id of the first account created in the first batch
            var id = results2[0][0].Id;
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
            Assert.AreEqual(results3[0].Count, 1);
            Assert.AreEqual(results3[0][0].Id, id);
            Assert.IsFalse(results3[0][0].Created);
            Assert.IsTrue(results3[0][0].Success);

            // create an Id list for the original strongly typed accounts created
            var idBatch = new SObjectList<SObject>();
            idBatch.AddRange(results1[0].Select(result => new SObject { { "Id", result.Id } }));

            // delete all the strongly typed accounts
            var results4 = await _client.RunJobAndPollAsync("Account", BulkConstants.OperationType.Delete,
                    new List<SObjectList<SObject>> { idBatch });

            Assert.IsTrue(results4 != null, "[results4] empty result object");
            Assert.AreEqual(results4.Count, 1, "[results4] wrong number of results");
            Assert.AreEqual(results4[0].Count, 3, "[results4] wrong number of result records");
            Assert.IsFalse(results4[0][0].Created);
            Assert.IsTrue(results4[0][0].Success);
        }
    }

}
