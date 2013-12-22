using System.Configuration;
using ForceSDKforNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    class Program
    {
        private static string _securityToken = ConfigurationSettings.AppSettings["SecurityToken"];
        private static string _consumerKey = ConfigurationSettings.AppSettings["ConsumerKey"];
        private static string _consumerSecret = ConfigurationSettings.AppSettings["ConsumerSecret"];
        private static string _username = ConfigurationSettings.AppSettings["Username"];
        private static string _password = ConfigurationSettings.AppSettings["Password"] + _securityToken;

        static void Main(string[] args)
        {
            BaseConstructor().Wait();
            AuthInConstructor().Wait();
            CreateTypedObject().Wait();
            CreateUntypedObject().Wait();
            CreateUpdateTypedObject().Wait();
            CreateDeleteTypedObject().Wait();
        }

        static async Task BaseConstructor()
        {
            try
            {
                var client = new ForceClient();

                await client.Authenticate(_consumerKey, _consumerSecret, _username, _password);
                var accounts = await client.Query<Account>("SELECT id, name, description FROM Account");

                Console.WriteLine(accounts.Count);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        static async Task AuthInConstructor()
        {
            try
            {
                var client = new ForceClient(_consumerKey, _consumerSecret, _username, _password);
                var accounts = await client.Query<Account>("SELECT id, name, description FROM Account");

                Console.WriteLine(accounts.Count);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        static async Task CreateTypedObject()
        {
            try
            {
                var client = new ForceClient(_consumerKey, _consumerSecret, _username, _password);
                var account = new Account() {Name = "New Name", Description = "New Description"};
                var id = await client.Create("Account", account);

                Console.WriteLine(id);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        static async Task CreateUntypedObject()
        {
            try
            {
                var client = new ForceClient(_consumerKey, _consumerSecret, _username, _password);
                var account = new { Name = "New Name", Description = "New Description" };
                var id = await client.Create("Account", account);

                Console.WriteLine(id);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }


        static async Task CreateUpdateTypedObject()
        {
            try
            {
                var client = new ForceClient(_consumerKey, _consumerSecret, _username, _password);
                var account = new Account() { Name = "New Name", Description = "New Description" };
                var id = await client.Create("Account", account);

                account.Name = "New Name 2";

                var success = await client.Update("Account", id, account);

                if (success)
                {
                    //TODO: add check by searching for ID and confirming
                }

                Console.WriteLine(id);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        static async Task CreateDeleteTypedObject()
        {
            try
            {
                var client = new ForceClient(_consumerKey, _consumerSecret, _username, _password);

                var account = new Account() { Name = "New Name", Description = "New Description" };
                var id = await client.Create("Account", account);
                var success = await client.Delete("Account", id);

                Console.WriteLine(success.ToString());
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
    }
}
