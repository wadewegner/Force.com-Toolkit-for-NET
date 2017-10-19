using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salesforce.Common;
using Salesforce.Force;

namespace CompositeConsole
{
    class Program
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
            var client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);

            //Create some Accounts using the SOobject Tree
            var sObjectTreeRecords = new
            {
                records = new[]
                {
                    new
                    {
                        attributes = new {type = "Account", referenceId = "ref1"},
                        name = "SampleAccount1",
                        phone = "1111111111",
                        website = "nicode.org",
                        numberOfEmployees = "1",
                        industry = "Software"
                    },
                    new
                    {
                        attributes = new {type = "Account", referenceId = "ref2"},
                        name = "SampleAccount2",
                        phone = "22222222222",
                        website = "nicode.org",
                        numberOfEmployees = "2",
                        industry = "Software"
                    },
                    new
                    {
                        attributes = new {type = "Account", referenceId = "ref3"},
                        name = "SampleAccount1",
                        phone = "3333333333",
                        website = "nicode.org",
                        numberOfEmployees = "3",
                        industry = "Software"
                    },
                    new
                    {
                        attributes = new {type = "Account", referenceId = "ref4"},
                        name = "SampleAccount4",
                        phone = "4444444444",
                        website = "nicode.org",
                        numberOfEmployees = "4",
                        industry = "Software"
                    },
                }
            };

            var results = (await client.SObjectTreeSave("Account", sObjectTreeRecords)).results;

            //Record the AccountIds to a List
            var newAccountIds = new List<string>();
            foreach (var successResponseObjectTreeResult in results)
            {
                Console.WriteLine(successResponseObjectTreeResult.referenceId + " : " + successResponseObjectTreeResult.id);
                newAccountIds.Add(successResponseObjectTreeResult.id);
            }


            //Using the partition extension method to split into partitions of 25 (requirement for batch request)
            foreach (var newAccountIdPartition in newAccountIds.Partition(25))
            {
                //Update the industry of each Account
                var updateObject = new
                {
                    batchRequests = newAccountIdPartition
                        .Select(id => new
                        {
                            method = "PATCH",
                            url = "v34.0/sobjects/Account/" + id,
                            richInput = new { Industry = "Engineering" }
                        })
                        .ToArray()
                };
                var unused = (await client.BatchSave(updateObject)).results;
            }

            //Verifying industry changed
            var accounts = (await client.QueryAsync<Account>("SELECT Id, Industry FROM Account WHERE website = 'nicode.org'")).Records;
            foreach (var account in accounts)
            {
                Console.WriteLine(account.Id + " : " + account.Industry);
            }

            foreach (var accountPartition in accounts.Partition(25))
            {
                var deleteObjects = new
                {
                    batchRequests = accountPartition
                        .Select(account => new { method = "DELETE", url = "v34.0/sobjects/Account/" + account.Id })
                        .ToArray()
                };
                var unused = (await client.BatchSave(deleteObjects)).results;
            }
        }
        public class Account
        {
            public string Id { get; set; }
            public string Industry { get; set; }
        }
    }
}
