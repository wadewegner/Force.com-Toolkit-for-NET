using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Salesforce.Common;
using Salesforce.Common.Models;
using Salesforce.Force;
using System.Threading.Tasks;
using System.Dynamic;

namespace SimpleConsole
{
    class Program
    {
#pragma warning disable 618
        private static readonly string SecurityToken = ConfigurationSettings.AppSettings["SecurityToken"];
        private static readonly string ConsumerKey = ConfigurationSettings.AppSettings["ConsumerKey"];
        private static readonly string ConsumerSecret = ConfigurationSettings.AppSettings["ConsumerSecret"];
        private static readonly string Username = ConfigurationSettings.AppSettings["Username"];
        private static readonly string Password = ConfigurationSettings.AppSettings["Password"] + SecurityToken;
        private static readonly string IsSandboxUser = ConfigurationSettings.AppSettings["IsSandboxUser"];
#pragma warning restore 618

        static void Main()
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
        }

        private static async Task RunSample()
        {
            var auth = new AuthenticationClient();

            // Authenticate with Salesforce
            Console.WriteLine("Authenticating with Salesforce");
            var url = IsSandboxUser.Equals("true", StringComparison.CurrentCultureIgnoreCase)
                ? "https://test.salesforce.com/services/oauth2/token"
                : "https://login.salesforce.com/services/oauth2/token";

            await auth.UsernamePasswordAsync(ConsumerKey, ConsumerSecret, Username, Password, ".net-api-client", url);
            Console.WriteLine("Connected to Salesforce");

            var client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);

            // retrieve all accounts
            Console.WriteLine("Get Accounts");
            var qry = "SELECT ID, Name FROM Account";
            var accts = new List<Account>();
            var totalSize = 0;
            
            QueryResult<Account> results = await client.QueryAsync<Account>(qry);
            totalSize = results.totalSize;
            Console.WriteLine("Queried " + totalSize + " records.");

            accts.AddRange(results.records);
            var nextRecordsUrl = results.nextRecordsUrl;

            if (!string.IsNullOrEmpty(nextRecordsUrl))
            {
                Console.WriteLine("Found nextRecordsUrl.");

                while (true)
                {
                    QueryResult<Account> continuationResults = await client.QueryContinuationAsync<Account>(nextRecordsUrl);
                    totalSize = continuationResults.totalSize;
                    Console.WriteLine("Queried an additional " + totalSize + " records.");

                    accts.AddRange(continuationResults.records);
                    if (string.IsNullOrEmpty(continuationResults.nextRecordsUrl)) break;

                    //pass nextRecordsUrl back to client.QueryAsync to request next set of records
                    nextRecordsUrl = continuationResults.nextRecordsUrl;
                }
            }
            Console.WriteLine("Retrieved accounts = " + accts.Count() + ", expected size = " + totalSize);

            // Create a sample record
            Console.WriteLine("Creating test record.");
            var account = new Account { Name = "Test Account" };
            account.Id = await client.CreateAsync(Account.SObjectTypeName, account);
            if (account.Id == null)
            {
                Console.WriteLine("Failed to create test record.");
                return;
            }

            Console.WriteLine("Successfully created test record.");

            // Update the sample record
            // Shows that annonymous types can be used as well
            Console.WriteLine("Updating test record.");
            var success = await client.UpdateAsync(Account.SObjectTypeName, account.Id, new { Name = "Test Update" });
            if (!success)
            {
                Console.WriteLine("Failed to update test record!");
                return;
            }

            Console.WriteLine("Successfully updated the record.");

            // Retrieve the sample record
            // How to retrieve a single record if the id is known
            Console.WriteLine("Retrieving the record by ID.");
            account = await client.QueryByIdAsync<Account>(Account.SObjectTypeName, account.Id);
            if (account == null)
            {
                Console.WriteLine("Failed to retrieve the record by ID!");
                return;
            }

            Console.WriteLine("Retrieved the record by ID.");

            // Query for record by name
            Console.WriteLine("Querying the record by name.");
            var accounts = await client.QueryAsync<Account>("SELECT ID, Name FROM Account WHERE Name = '" + account.Name + "'");
            account = accounts.records.FirstOrDefault();
            if (account == null)
            {
                Console.WriteLine("Failed to retrieve account by query!");
                return;
            }

            Console.WriteLine("Retrieved the record by name.");

            // Delete account
            Console.WriteLine("Deleting the record by ID.");
            success = await client.DeleteAsync(Account.SObjectTypeName, account.Id);
            if (!success)
            {
                Console.WriteLine("Failed to delete the record by ID!");
                return;
            }
            Console.WriteLine("Deleted the record by ID.");

            // Selecting multiple accounts into a dynamic
            Console.WriteLine("Querying multiple records.");
            var dynamicAccounts = await client.QueryAsync<dynamic>("SELECT ID, Name FROM Account LIMIT 10");
            foreach (dynamic acct in dynamicAccounts.records)
            {
                Console.WriteLine("Account - " + acct.Name);
            }

            // Creating parent - child records using a Dynamic
            Console.WriteLine("Creating a parent record (Account)");
            dynamic a = new ExpandoObject();
            a.Name = "Account from .Net Toolkit";
            a.Id = await client.CreateAsync("Account", a);
            if (a.Id == null)
            {
                Console.WriteLine("Failed to create parent record.");
                return;
            }

            Console.WriteLine("Creating a child record (Contact)");
            dynamic c = new ExpandoObject();
            c.FirstName = "Joe";
            c.LastName = "Blow";
            c.AccountId = a.Id;
            c.Id = await client.CreateAsync("Contact", c);
            if (c.Id == null)
            {
                Console.WriteLine("Failed to create child record.");
                return;
            }

            Console.WriteLine("Press Enter to delete parent and child and continue");
            Console.Read();

            // Delete account (also deletes contact)
            Console.WriteLine("Deleting the Account by Id.");
            success = await client.DeleteAsync(Account.SObjectTypeName, a.Id);
            if (!success)
            {
                Console.WriteLine("Failed to delete the record by ID!");
                return;
            }
            Console.WriteLine("Deleted the Account and Contact.");

        }

        private class Account
        {
            public const String SObjectTypeName = "Account";

            public String Id { get; set; }
            public String Name { get; set; }
        }
    }
}
