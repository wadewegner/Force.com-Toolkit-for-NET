using System;
using System.Linq;
using CredentialManagement;
using Salesforce.Common;
using Salesforce.Force;
using System.Threading.Tasks;

namespace SimpleConsole
{
    class Program
    {
        // this sample uses the CredentialManagement NuGet package to load credentials and
        // tokens from the Windows Credential Vault. To set this up, open the vault and add
        // two entries with the keys you specify below.
        // CRED_KEY is for your username and password
        // TOKEN_KEY is for the consumer token and secret

        // this is used to load the username/password from the windows credential vault
        private const String CRED_KEY = "ndrees@23demo.com";
        // this is used to load the consumer 
        private const String TOKEN_KEY = "ndrees@23demo.com-token";

        static void Main(string[] args)
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

                Exception innerException = e.InnerException;
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

            // load credentials from credential vault
            // this uses the CredentialManagement NuGet package
            using (var cred = new Credential { Target = CRED_KEY })
            using (var token = new Credential { Target = TOKEN_KEY })
            {
                if (!cred.Load())
                    throw new ArgumentException(CRED_KEY);
                if (!token.Load())
                    throw new ArgumentException(TOKEN_KEY);

                // authenticate with salesforce
                Console.WriteLine("Authenticating with Salesforce");
                await auth.UsernamePassword(token.Username, token.Password, cred.Username, cred.Password);
                Console.WriteLine("Connected to Salesforce");
            }

            var client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);
            
            // Create a sample record
            Console.WriteLine("Creating test account.");
            var account = new Account { Name = "Test Account" };
            account.Id = await client.Create(Account.SObjectTypeName, account);
            if (account.Id == null)
            {
                Console.WriteLine("Failed to create test account.");
                return;
            }

            // Update the sample record
            // shows that annonymous types can be used as well
            var success = await client.Update(Account.SObjectTypeName, account.Id, new { Name = "Test Update" });
            if (!success)
                Console.WriteLine("Failed to update account!");

            // Retrieve the sample record
            // how to retrieve a single record if the id is known
            account = await client.QueryById<Account>(Account.SObjectTypeName, account.Id);
            if (account == null)
                Console.WriteLine("Failed to retrieve account be id!");

            // Query for record by name
            var accounts = await client.Query<Account>("SELECT ID, Name FROM Account WHERE Name = '" + account.Name + "'");
            account = accounts.FirstOrDefault();
            if (account == null)
                Console.WriteLine("Failed to retrieve account by query!");

            // Delete account
            success = await client.Delete(Account.SObjectTypeName, account.Id);
            if (!success)
                Console.WriteLine("Failed to delete account record!");
        }

        private class Account
        {
            public const String SObjectTypeName = "Account";

            public String Id { get; set; }
            public String Name { get; set; }
        }
    }
}
