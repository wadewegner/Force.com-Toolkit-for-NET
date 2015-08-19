using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Salesforce.Common;
using Salesforce.Force.Bulk;
using Salesforce.Force.Bulk.Models;

namespace AdvancedBulkConsole
{
    public class Program
    {
        private static readonly string SecurityToken = ConfigurationManager.AppSettings["SecurityToken"];
        private static readonly string ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
        private static readonly string ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
        private static readonly string Username = ConfigurationManager.AppSettings["Username"];
        private static readonly string Password = ConfigurationManager.AppSettings["Password"] + SecurityToken;
        private static readonly string IsSandboxUser = ConfigurationManager.AppSettings["IsSandboxUser"];

        public static void Main()
        {
            try
            {
                var task = RunSample();
                task.Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);

                var innerException = e.InnerException;
                while (innerException != null)
                {
                    Console.WriteLine(innerException.Message);
                    Console.WriteLine(innerException.StackTrace);

                    innerException = innerException.InnerException;
                }
            }

            Console.WriteLine("\nPress enter to close...");
            Console.ReadLine();
        }

        private static async Task RunSample()
        {
            var auth = new AuthenticationClient();

            // Authenticate with Salesforce
            Console.WriteLine("Authenticating with Salesforce");
            var url = IsSandboxUser.Equals("true", StringComparison.CurrentCultureIgnoreCase)
                ? "https://test.salesforce.com/services/oauth2/token"
                : "https://login.salesforce.com/services/oauth2/token";

            await auth.UsernamePasswordAsync(ConsumerKey, ConsumerSecret, Username, Password, url);
            Console.WriteLine("Connected to Salesforce");

            // Get a bulk client
            var client = new BulkForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);

            // create a job
            var jobInfo = await client.CreateJobAsync("Account", Bulk.OperationType.Insert);
            Console.WriteLine("Created a Job");

            // Make a dynamic typed Account list
            var dtAccountsBatch1 = new SObjectList<SObject>
            {
                new SObject{{"Name", "TestDtAccount1"}},
                new SObject{{"Name", "TestDtAccount2"}},
                new SObject{{"MADEUPFIELD", "MADEUPVALUE"}},
                new SObject{{"Name", "TestDtAccount3"}}
            };

            // Make a dynamic typed Account list
            var dtAccountsBatch2 = new SObjectList<SObject>
            {
                new SObject{{"Name", "TestDtAccount4"}},
                new SObject{{"Name", "TestDtAccount5"}},
                new SObject{{"Name", "TestDtAccount6"}}
            };

            // Make a dynamic typed Account list
            var dtAccountsBatch3 = new SObjectList<SObject>
            {
                new SObject{{"MADEUPFIELD", "MADEUPVALUE"}},
                new SObject{{"Name", "TestDtAccount7"}},
                new SObject{{"Name", "TestDtAccount8"}},
                new SObject{{"Name", "TestDtAccount9"}}
            };

            // create the batches
            var batchInfoList = new List<BatchInfoResult>
            {
                await client.CreateJobBatchAsync(jobInfo, dtAccountsBatch1),
                await client.CreateJobBatchAsync(jobInfo, dtAccountsBatch2),
                await client.CreateJobBatchAsync(jobInfo, dtAccountsBatch3)
            };
            Console.WriteLine("Created Three Batches");

            // poll
            var pollStart = 1.0f;
            const float pollIncrease = 2.0f;
            var results = new List<BatchResultList>();
            while (batchInfoList.Count > 0)
            {
                var removeList = new List<BatchInfoResult>();
                foreach (var batchInfo in batchInfoList)
                {
                    var newBatchInfo = await client.PollBatchAsync(batchInfo);
                    if (newBatchInfo.State.Equals(Bulk.BatchState.Completed.Value()) ||
                        newBatchInfo.State.Equals(Bulk.BatchState.Failed.Value()) ||
                        newBatchInfo.State.Equals(Bulk.BatchState.NotProcessed.Value()))
                    {
                        results.Add(await client.GetBatchResult(newBatchInfo));
                        removeList.Add(batchInfo);
                    }
                }
                foreach (var removeInfo in removeList)
                {
                    batchInfoList.Remove(removeInfo);
                }
                await Task.Delay((int) pollStart);
                pollStart *= pollIncrease;
            }

            Console.WriteLine("All Batches Complete, \"var results\" contains the result objects (Each is a list containing a result for each record):");
            foreach (var result in results.SelectMany(resultList => resultList))
            {
                Console.WriteLine("Id:{0}, Created:{1}, Success:{2}, Errors:{3}", result.Id, result.Created, result.Success, result.Errors != null);
                if (result.Errors != null)
                {
                    Console.WriteLine("\tErrors:");
                    var resultErrors = result.Errors;
                    foreach (var field in resultErrors.Fields)
                    {
                        Console.WriteLine("\tField:{0}", field);
                    }
                    Console.WriteLine("\t{0}",resultErrors.Message);
                    Console.WriteLine("\t{0}",resultErrors.StatusCode);
                }
            }

        }
    }
}
