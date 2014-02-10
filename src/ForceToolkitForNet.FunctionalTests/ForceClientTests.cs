//TODO: add license header

using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Salesforce.Common;
using Salesforce.Force.FunctionalTests.Models;

namespace Salesforce.Force.FunctionalTests
{
    [TestFixture]
    public class ForceClientTests
    {
#pragma warning disable 618
        private static string _tokenRequestEndpointUrl = ConfigurationSettings.AppSettings["TokenRequestEndpointUrl"];
        private static string _securityToken = ConfigurationSettings.AppSettings["SecurityToken"];
        private static string _consumerKey = ConfigurationSettings.AppSettings["ConsumerKey"];
        private static string _consumerSecret = ConfigurationSettings.AppSettings["ConsumerSecret"];
        private static string _username = ConfigurationSettings.AppSettings["Username"];
        private static string _password = ConfigurationSettings.AppSettings["Password"] + _securityToken;
#pragma warning restore 618

        public async Task<ForceClient> GetForceClient(HttpClient httpClient)
        {
            var auth = new AuthenticationClient(httpClient);
            await auth.UsernamePassword(_consumerKey, _consumerSecret, _username, _password);

            var client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion, httpClient);
            return client;
        }

        [Test]
        public async void Query_Accounts_IsNotEmpty()
        {
            using (var httpClient = new HttpClient())
            {
                var client = await GetForceClient(httpClient);
                var accounts = await client.Query<Account>("SELECT id, name, description FROM Account");

                Assert.IsNotNull(accounts);
            }
        }

        [Test]
        public async void Query_Accounts_BadObject()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var client = await GetForceClient(httpClient);
                    await client.Query<Account>("SELECT id, name, description FROM BadObject");
                }
            }
            catch (ForceException ex)
            {
                Assert.IsNotNull(ex);
                Assert.IsNotNull(ex.Message);
                Assert.IsNotNull(ex.Error);
            }
        }

        [Test]
        public async void Create_Account_Typed()
        {
            using (var httpClient = new HttpClient())
            {
                var client = await GetForceClient(httpClient);
                var account = new Account { Name = "New Account", Description = "New Account Description" };
                var id = await client.Create("Account", account);

                Assert.IsNotNullOrEmpty(id);
            }
        }

        [Test]
        public async void Create_Account_Untyped()
        {
            using (var httpClient = new HttpClient())
            {
                var client = await GetForceClient(httpClient);
                var account = new { Name = "New Account", Description = "New Account Description" };
                var id = await client.Create("Account", account);

                Assert.IsNotNullOrEmpty(id);
            }
        }

        [Test]
        public async void Create_Account_Untyped_BadObject()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var client = await GetForceClient(httpClient);
                    var account = new { Name = "New Account", Description = "New Account Description" };
                    await client.Create("BadAccount", account);
                }
            }
            catch (ForceException ex)
            {
                Assert.IsNotNull(ex);
                Assert.IsNotNull(ex.Message);
                Assert.IsNotNull(ex.Error);
            }
        }

        [Test]
        public async void Create_Account_Untyped_BadFields()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var client = await GetForceClient(httpClient);
                    var account = new { BadName = "New Account", BadDescription = "New Account Description" };
                    await client.Create("Account", account);
                }
            }
            catch (ForceException ex)
            {
                Assert.IsNotNull(ex);
                Assert.IsNotNull(ex.Message);
                Assert.IsNotNull(ex.Error);
            }
        }

        [Test]
        public async void Update_Account_IsSuccess()
        {
            using (var httpClient = new HttpClient())
            {
                var client = await GetForceClient(httpClient);

                var originalName = "New Account";
                var newName = "New Account 2";

                var account = new Account { Name = originalName, Description = "New Account Description" };
                var id = await client.Create("Account", account);

                account.Name = newName;

                var success = await client.Update("Account", id, account);

                Assert.IsTrue(success);
            }
        }

        [Test]
        public async void Update_Account_BadObject()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var client = await GetForceClient(httpClient);

                    var originalName = "New Account";
                    var newName = "New Account 2";

                    var account = new Account { Name = originalName, Description = "New Account Description" };
                    var id = await client.Create("Account", account);

                    account.Name = newName;

                    await client.Update("BadAccount", id, account);
                }
            }
            catch (ForceException ex)
            {
                Assert.IsNotNull(ex);
                Assert.IsNotNull(ex.Message);
                Assert.IsNotNull(ex.Error);
            }
        }

        [Test]
        public async void Update_Account_BadField()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var client = await GetForceClient(httpClient);

                    var originalName = "New Account";
                    var newName = "New Account 2";

                    var account = new { Name = originalName, Description = "New Account Description" };
                    var id = await client.Create("Account", account);

                    var updatedAccount = new { BadName = newName, Description = "New Account Description" };

                    await client.Update("Account", id, updatedAccount);
                }
            }
            catch (ForceException ex)
            {
                Assert.IsNotNull(ex);
                Assert.IsNotNull(ex.Message);
                Assert.IsNotNull(ex.Error);
            }
        }

        [Test]
        public async void Update_Account_NameChanged()
        {
            using (var httpClient = new HttpClient())
            {
                var client = await GetForceClient(httpClient);

                var originalName = "New Account";
                var newName = "New Account 2";

                var account = new Account { Name = originalName, Description = "New Account Description" };
                var id = await client.Create("Account", account);
                account.Name = newName;
                await client.Update("Account", id, account);

                var result = await client.QueryById<Account>("Account", id);

                Assert.True(result.Name == newName);
            }
        }

        [Test]
        public async void Delete_Account_IsSuccess()
        {
            using (var httpClient = new HttpClient())
            {
                var client = await GetForceClient(httpClient);
                var account = new Account { Name = "New Account", Description = "New Account Description" };
                var id = await client.Create("Account", account);
                var success = await client.Delete("Account", id);

                Assert.IsTrue(success);
            }
        }

        [Test]
        public async void Delete_Account_ObjectDoesNotExist()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var client = await GetForceClient(httpClient);
                    var account = new Account { Name = "New Account", Description = "New Account Description" };
                    var id = await client.Create("Account", account);
                    var success = await client.Delete("BadAccount", id);

                    Assert.IsTrue(success);
                }
            }
            catch (ForceException ex)
            {
                Assert.IsNotNull(ex);
                Assert.IsNotNull(ex.Message);
                Assert.IsNotNull(ex.Error);
            }
        }

        [Test]
        public async void Delete_Account_IdDoesNotExist()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var client = await GetForceClient(httpClient);
                    var id = "asdfasdfasdf";
                    await client.Delete("Account", id);
                }
            }
            catch (ForceException ex)
            {
                Assert.IsNotNull(ex);
                Assert.IsNotNull(ex.Message);
                Assert.IsNotNull(ex.Error);
            }
        }

        [Test]
        public async void Delete_Account_ValidateIsGone()
        {
            using (var httpClient = new HttpClient())
            {
                var client = await GetForceClient(httpClient);
                var account = new Account { Name = "New Account", Description = "New Account Description" };
                var id = await client.Create("Account", account);
                await client.Delete("Account", id);

                var result = await client.QueryById<Account>("Account", id);

                Assert.IsNull(result);
            }
        }

        [Test]
        public async void Objects_GetAllObjects_IsNotNull()
        {
            using (var httpClient = new HttpClient())
            {
                var client = await GetForceClient(httpClient);
                var objects = await client.GetObjects<object>();

                Assert.IsNotNull(objects);
            }
        }

        [Test]
        public async void Object_Describe_IsNotNull()
        {
            using (var httpClient = new HttpClient())
            {
                var client = await GetForceClient(httpClient);
                var accounts = await client.Describe<object>("Account");

                Assert.IsNotNull(accounts);
            }
        }

        [Test]
        public async void Recent_IsNotNull()
        {
            using (var httpClient = new HttpClient())
            {
                var client = await GetForceClient(httpClient);
                var recent = await client.Recent<object>(5);

                Assert.IsNotNull(recent);
            }
        }
    }
}
