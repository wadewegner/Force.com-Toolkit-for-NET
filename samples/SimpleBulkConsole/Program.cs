using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Salesforce.Common;
using Salesforce.Force.Bulk;
using Salesforce.Force.Bulk.Models;

namespace SimpleBulkConsole
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

            // Make a strongly typed Account list
            var stAccountsBatch = new SObjectList<Account>
            {
                new Account {Name = "TestStAccount1"},
                new Account {Name = "TestStAccount2"},
                new Account {Name = "TestStAccount3"}
            };

            // insert the accounts
            var results1 = await client.RunJobAndPollAsync("Account", Bulk.OperationType.Insert,
                    new List<SObjectList<Account>>{stAccountsBatch});
            // (one SObjectList<T> per batch, the example above uses one batch)

            Console.WriteLine("Strongly typed accounts created");

            // Make a dynamic typed Account list
            var dtAccountsBatch = new SObjectList<SObject>
            {
                new SObject{{"Name", "TestDtAccount1"}},
                new SObject{{"Name", "TestDtAccount2"}},
                new SObject{{"Name", "TestDtAccount3"}}
            };

            // insert the accounts
            var results2 = await client.RunJobAndPollAsync("Account", Bulk.OperationType.Insert,
                    new List<SObjectList<SObject>>{dtAccountsBatch});

            Console.WriteLine("Dynamically typed accounts created");

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
            var results3 = await client.RunJobAndPollAsync("Account", Bulk.OperationType.Update,
                    new List<SObjectList<SObject>>{dtAccountsBatch});

            Console.WriteLine("Account with ID {0} updated", id);

            // create an Id list for the original strongly typed accounts created
            var idBatch = new SObjectList<SObject>();
            idBatch.AddRange(results1[0].Select(result => new SObject {{"Id", result.Id}}));

            // delete all the strongly typed accounts
            var results4 = await client.RunJobAndPollAsync("Account", Bulk.OperationType.Delete,
                    new List<SObjectList<SObject>>{idBatch});

            Console.WriteLine("Accounts deleted");
        }

        public class Account
        {
            public string Name { get; set; }
        }
    }
}
