using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using Salesforce.Force.Bulk;
using Salesforce.Force.Bulk.Models;

namespace Salesforce.Force.UnitTests
{
    [TestFixture]
    public class BulkForceClientTests
    {
        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public async void RunJobAndPoll_NullObjectName_ArgumentNullException()
        {
            var client = new BulkForceClient("test", "test", "v32");
            await client.RunJobAndPoll(null, Bulk.Bulk.OperationType.Insert, new List<ISObjectList<SObject>>());

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void RunJobAndPoll_NullBatchList_ArgumentNullException()
        {
            var client = new BulkForceClient("test", "test", "v32");
            await client.RunJobAndPoll<List<ISObjectList<SObject>>>("Account", Bulk.Bulk.OperationType.Insert, null);

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void RunJob_NullObjectName_ArgumentNullException()
        {
            var client = new BulkForceClient("test", "test", "v32");
            await client.RunJob(null, Bulk.Bulk.OperationType.Insert, new List<ISObjectList<SObject>>());

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void RunJob_NullBatchList_ArgumentNullException()
        {
            var client = new BulkForceClient("test", "test", "v32");
            await client.RunJob<List<ISObjectList<SObject>>>("Account", Bulk.Bulk.OperationType.Insert, null);

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void CreateJobAsync_NullObjectName_ArgumentNullException()
        {
            var client = new BulkForceClient("test", "test", "v32");
            await client.CreateJobAsync(null, Bulk.Bulk.OperationType.Insert);

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void CreateJobBatchAsync_NullJobId_ArgumentNullException()
        {
            var client = new BulkForceClient("test", "test", "v32");
            await client.CreateJobBatchAsync((string)null, new SObjectList<SObject>());

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void CreateJobBatchAsync_NullJobInfo_ArgumentNullException()
        {
            var client = new BulkForceClient("test", "test", "v32");
            await client.CreateJobBatchAsync((JobInfoResult)null, new SObjectList<SObject>());

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void CreateJobBatchAsync_NullObjectList_ArgumentNullException()
        {
            var client = new BulkForceClient("test", "test", "v32");
            await client.CreateJobBatchAsync<SObjectList<SObject>>("testId", null);

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void PollJobAsync_NullJobInfo_ArgumentNullException()
        {
            var client = new BulkForceClient("test", "test", "v32");
            await client.PollJobAsync((JobInfoResult)null);

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void PollJobAsync_NullJobId_ArgumentNullException()
        {
            var client = new BulkForceClient("test", "test", "v32");
            await client.PollJobAsync((string)null);

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void PollBatchAsync_NullBatchInfo_ArgumentNullException()
        {
            var client = new BulkForceClient("test", "test", "v32");
            await client.PollBatchAsync(null);

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void PollBatchAsync_NullBatchId_ArgumentNullException()
        {
            var client = new BulkForceClient("test", "test", "v32");
            await client.PollBatchAsync(null, "test");

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void PollBatchAsync_NullJobId_ArgumentNullException()
        {
            var client = new BulkForceClient("test", "test", "v32");
            await client.PollBatchAsync("test", null);

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void GetBatchResultAsync_NullBatchInfo_ArgumentNullException()
        {
            var client = new BulkForceClient("test", "test", "v32");
            await client.GetBatchResult(null);

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void GetBatchResultAsync_NullBatchId_ArgumentNullException()
        {
            var client = new BulkForceClient("test", "test", "v32");
            await client.GetBatchResult(null, "test");

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void GetBatchResultAsync_NullJobId_ArgumentNullException()
        {
            var client = new BulkForceClient("test", "test", "v32");
            await client.GetBatchResult("test", null);

            // expects exception
        }

        /// <summary>
        /// This method will test all of the child methods in turn,
        /// It means that other methods dont need testing.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void RunJobAndPoll_GoodData_CompleteRunThroughSucceeds()
        {
            var expectedResponses = new List<HttpResponseMessage>
            {
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.FromFile("KnownGoodContent/UserObjectDescribeMetadata.json")
                }
            };

            var testingActions = new List<Action<HttpRequestMessage>>
            {
                r => { },
                r => { }
            };

            var inputList = new List<SObjectList<SObject>>
            {
                new SObjectList<SObject>
                {
                    new SObject
                    {
                        {"Name", "TestAccount1"}
                    }
                }
            };

            var httpClient = new HttpClient(new BulkFakeHttpRequestHandler(expectedResponses, testingActions));
            var client = new BulkForceClient("http://localhost:1899", "accessToken", "v29", httpClient);

            using (client)
            {
                await client.RunJobAndPoll("Account", Bulk.Bulk.OperationType.Insert, inputList);
            }
        }
    }
}
