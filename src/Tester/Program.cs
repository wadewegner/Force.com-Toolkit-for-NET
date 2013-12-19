using ForceSDKforNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    public class Account
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }


    class Program
    {
        static void Main(string[] args)
        {
            MainTask().Wait();

        }

        static async Task MainTask()
        {
            ForceRestClient client = new ForceRestClient();

            string consumerKey = "3MVG9A2kN3Bn17hsEyMqRTTaEfT8PtpprMk4qQoUe0ep4brWttwhxV1kRg5KB2dW2Hs4kWExau.h8VLEFeo37";
            string consumerSecret = "2486043778523914463";
            string username = "wade@sfdcapi.com";
            string password = "Passw0rd!";

            await client.Authenticate(consumerKey, consumerSecret, username, password);

            var accounts = await client.Query<Account>("SELECT id, name, description FROM Account");

            Console.WriteLine(accounts.Count);

        }
    }
}
