using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using Salesforce.Common.Models.Xml;

namespace Salesforce.Force.UnitTests
{
    [TestFixture]
    public class BulkForceClientTests
    {
        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public async void RunJobAndPoll_NullObjectName_ArgumentNullException()
        {
	        IForceClient client = new ForceClient("test", "test", "v32");
            await client.RunJobAndPollAsync(null, BulkConstants.OperationType.Insert, new List<ISObjectList<SObject>>());

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void RunJobAndPoll_NullBatchList_ArgumentNullException()
        {
	        IForceClient client = new ForceClient("test", "test", "v32");
            await client.RunJobAndPollAsync<List<ISObjectList<SObject>>>("Account", BulkConstants.OperationType.Insert, null);

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void RunJob_NullObjectName_ArgumentNullException()
        {
	        IForceClient client = new ForceClient("test", "test", "v32");
            await client.RunJobAsync(null, BulkConstants.OperationType.Insert, new List<ISObjectList<SObject>>());

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void RunJob_NullBatchList_ArgumentNullException()
        {
	        IForceClient client = new ForceClient("test", "test", "v32");
            await client.RunJobAsync<List<ISObjectList<SObject>>>("Account", BulkConstants.OperationType.Insert, null);

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void CreateJobAsync_NullObjectName_ArgumentNullException()
        {
	        IForceClient client = new ForceClient("test", "test", "v32");
            await client.CreateJobAsync(null, BulkConstants.OperationType.Insert);

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void CreateJobBatchAsync_NullJobId_ArgumentNullException()
        {
	        IForceClient client = new ForceClient("test", "test", "v32");
            await client.CreateJobBatchAsync((string)null, new SObjectList<SObject>());

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void CreateJobBatchAsync_NullJobInfo_ArgumentNullException()
        {
	        IForceClient client = new ForceClient("test", "test", "v32");
            await client.CreateJobBatchAsync((JobInfoResult)null, new SObjectList<SObject>());

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void CreateJobBatchAsync_NullObjectList_ArgumentNullException()
        {
	        IForceClient client = new ForceClient("test", "test", "v32");
            await client.CreateJobBatchAsync<SObjectList<SObject>>("testId", null);

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void PollJobAsync_NullJobInfo_ArgumentNullException()
        {
	        IForceClient client = new ForceClient("test", "test", "v32");
            await client.PollJobAsync((JobInfoResult)null);

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void PollJobAsync_NullJobId_ArgumentNullException()
        {
	        IForceClient client = new ForceClient("test", "test", "v32");
            await client.PollJobAsync((string)null);

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void PollBatchAsync_NullBatchInfo_ArgumentNullException()
        {
	        IForceClient client = new ForceClient("test", "test", "v32");
            await client.PollBatchAsync(null);

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void PollBatchAsync_NullBatchId_ArgumentNullException()
        {
	        IForceClient client = new ForceClient("test", "test", "v32");
            await client.PollBatchAsync(null, "test");

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void PollBatchAsync_NullJobId_ArgumentNullException()
        {
	        IForceClient client = new ForceClient("test", "test", "v32");
            await client.PollBatchAsync("test", null);

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void GetBatchResultAsync_NullBatchInfo_ArgumentNullException()
        {
	        IForceClient client = new ForceClient("test", "test", "v32");
            await client.GetBatchResultAsync(null);

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void GetBatchResultAsync_NullBatchId_ArgumentNullException()
        {
	        IForceClient client = new ForceClient("test", "test", "v32");
            await client.GetBatchResultAsync(null, "test");

            // expects exception
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void GetBatchResultAsync_NullJobId_ArgumentNullException()
        {
	        IForceClient client = new ForceClient("test", "test", "v32");
            await client.GetBatchResultAsync("test", null);

            // expects exception
        }

        /// <summary>
        /// This method will test all of the child methods in turn,
        /// It means that other methods dont need testing.
        /// </summary>
        [Test]
        public async void RunJobAndPoll_GoodData_CompleteRunThroughSucceeds()
        {
            var expectedResponses = new List<HttpResponseMessage>
            {
                // create job
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new XmlContent(new JobInfoResult
                    {
                        ApiVersion = 32.0f,
                        ConcurrencyMode = "Parallel",
                        ContentType = "XML",
                        CreatedById = "test",
                        CreatedDate = DateTime.Now,
                        Id = "test",
                        NumberBatchesCompleted = 0,
                        NumberBatchesFailed = 0,
                        NumberBatchesInProgress = 0,
                        NumberBatchesQueued = 1,
                        SystemModstamp = DateTime.Now,
                        State = "Open"
                    })
                },
                // create batch
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new XmlContent(new BatchInfoResult
                    {
                        CreatedDate = DateTime.Now,
                        Id = "test",
                        JobId = "test",
                        NumberRecordsProcessed = 0,
                        State = "Queued",
                        SystemModstamp = DateTime.Now
                    })
                },
                // close job
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new XmlContent(new JobInfoResult
                    {
                        ApiVersion = 32.0f,
                        ConcurrencyMode = "Parallel",
                        ContentType = "XML",
                        CreatedById = "test",
                        CreatedDate = DateTime.Now,
                        Id = "test",
                        NumberBatchesCompleted = 0,
                        NumberBatchesFailed = 0,
                        NumberBatchesInProgress = 0,
                        NumberBatchesQueued = 1,
                        SystemModstamp = DateTime.Now,
                        State = "Closed"
                    })
                },
                // poll batch
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new XmlContent(new BatchInfoResult
                    {
                        CreatedDate = DateTime.Now,
                        Id = "test",
                        JobId = "test",
                        NumberRecordsProcessed = 1,
                        State = "Completed",
                        SystemModstamp = DateTime.Now
                    })
                },
                // get batch result
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new XmlContent(new BatchResultList
                    {
                        Items = new SObjectList<BatchResult>
                        {
                            new BatchResult
                            {
                                Id = "000000000000000000",
                                Created = true,
                                Success = true
                            }
                        }
                    })
                }
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

            var testingActions = new List<Action<HttpRequestMessage>>
            {
                // create job
                r => Assert.AreEqual(r.Content.ReadAsStringAsync().Result, MimicSerialization(new JobInfo
                {
                    ContentType = "XML",
                    Object = "Account",
                    Operation = BulkConstants.OperationType.Insert.Value()
                })),
                // create batch
                r => Assert.AreEqual(r.Content.ReadAsStringAsync().Result, MimicSerialization(inputList[0])),
                // close job
                r => Assert.AreEqual(r.Content.ReadAsStringAsync().Result, MimicSerialization(new JobInfoState
                {
                    State = "Closed"
                })),
                // poll batch
                r => { /* NO PAYLOAD */ },
                // get batch result
                r => { /* NO PAYLOAD */ }
            };

            var httpClient = new HttpClient(new BulkFakeHttpRequestHandler(expectedResponses, testingActions));
	        IForceClient client = new ForceClient("http://localhost:1899", "accessToken", "v29", new HttpClient(), httpClient);

            using (client)
            {
                await client.RunJobAndPollAsync("Account", BulkConstants.OperationType.Insert, inputList);
            }
        }

        private static string MimicSerialization(object inputObject)
        {
            var xmlSerializer = new XmlSerializer(inputObject.GetType());
            var stringWriter = new StringWriter();
            string result;
            using (var writer = XmlWriter.Create(stringWriter))
            {
                xmlSerializer.Serialize(writer, inputObject);
                result = stringWriter.ToString();
            }
            return result;
        }
    }
}
