﻿//TODO: add license header

using System.Configuration;
using System.Linq;
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
            await auth.UsernamePasswordAsync(_consumerKey, _consumerSecret, _username, _password);

            var client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion, httpClient);
            return client;
        }

        [Test]
        public async void Query_Count()
        {
            using (var httpClient = new HttpClient())
            {
                var client = await GetForceClient(httpClient);
                var accounts = await client.QueryAsync<Account>("SELECT count() FROM Account");

                Assert.IsNotNull(accounts);
            }
        }

        [Test]
        public async void Query_Accounts_IsNotEmpty()
        {
            using (var httpClient = new HttpClient())
            {
                var client = await GetForceClient(httpClient);
                var accounts = await client.QueryAsync<Account>("SELECT id, name, description FROM Account");

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
                    await client.QueryAsync<Account>("SELECT id, name, description FROM BadObject");
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
                var id = await client.CreateAsync("Account", account);

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
                var id = await client.CreateAsync("Account", account);

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
                    await client.CreateAsync("BadAccount", account);
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
                    var id = await client.CreateAsync("Account", account);
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
                var id = await client.CreateAsync("Account", account);

                account.Name = newName;

                var success = await client.UpdateAsync("Account", id, account);

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
                    var id = await client.CreateAsync("Account", account);

                    account.Name = newName;

                    await client.UpdateAsync("BadAccount", id, account);
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
                    var id = await client.CreateAsync("Account", account);

                    var updatedAccount = new { BadName = newName, Description = "New Account Description" };

                    await client.UpdateAsync("Account", id, updatedAccount);
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
                var id = await client.CreateAsync("Account", account);
                account.Name = newName;
                await client.UpdateAsync("Account", id, account);

                var result = await client.QueryByIdAsync<Account>("Account", id);

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
                var id = await client.CreateAsync("Account", account);
                var success = await client.DeleteAsync("Account", id);

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
                    var id = await client.CreateAsync("Account", account);
                    var success = await client.DeleteAsync("BadAccount", id);

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
                    await client.DeleteAsync("Account", id);
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
                var id = await client.CreateAsync("Account", account);
                await client.DeleteAsync("Account", id);

                var result = await client.QueryByIdAsync<Account>("Account", id);

                Assert.IsNull(result);
            }
        }

        [Test]
        public async void Objects_GetAllObjects_IsNotNull()
        {
            using (var httpClient = new HttpClient())
            {
                var client = await GetForceClient(httpClient);
                var objects = await client.GetObjectsAsync<object>();

                Assert.IsNotNull(objects);
            }
        }

        [Test]
        public async void Object_Describe_IsNotNull()
        {
            using (var httpClient = new HttpClient())
            {
                var client = await GetForceClient(httpClient);
                var accounts = await client.DescribeAsync<object>("Account");

                Assert.IsNotNull(accounts);
            }
        }

        [Test]
        public async void Recent_IsNotNull()
        {
            using (var httpClient = new HttpClient())
            {
                var client = await GetForceClient(httpClient);
                var recent = await client.RecentAsync<object>(5);

                Assert.IsNotNull(recent);
            }
        }

        [Test]
        //This test assumes that an external ID field, “ExternalID__c” has been added to Account. 
        public async void Upsert_Account_IsSuccess()
        {
            using (var httpClient = new HttpClient())
            {
                var client = await GetForceClient(httpClient);

                var account = new Account { Name = "New Account", Description = "New Account Description" };

                var success = await client.UpsertExternalAsync("Account", "ExternalID__c", "123", account);

                Assert.IsTrue(success);
            }
        }

        [Test]
        //This test assumes that an external ID field, “ExternalID__c” has been added to Account. 
        public async void Upsert_Account_BadObject()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var client = await GetForceClient(httpClient);

                    var account = new Account { Name = "New Account ExternalID", Description = "New Account Description" };

                    await client.UpsertExternalAsync("BadAccount", "ExternalID__c", "2", account);
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
        //This test assumes that an external ID field, “ExternalID__c” has been added to Account. 
        public async void Upsert_Account_BadField()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var client = await GetForceClient(httpClient);

                    var accountBadName = new { BadName = "New Account", Description = "New Account Description" };

                    await client.UpsertExternalAsync("Account", "ExternalID__c", "3", accountBadName);
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
        //This test assumes that an external ID field, “ExternalID__c,” has been added to Account. 
        public async void Upsert_Account_NameChanged()
        {
            using (var httpClient = new HttpClient())
            {
                var client = await GetForceClient(httpClient);

                var originalName = "New Account External Upsert";
                var newName = "New Account External Upsert 2";

                var account = new Account { Name = originalName, Description = "New Account Description" };
                await client.UpsertExternalAsync("Account", "ExternalID__c", "4", account);

                account.Name = newName;
                await client.UpsertExternalAsync("Account", "ExternalID__c", "4", account);

                var accountResult = await client.QueryAsync<Account>("SELECT Name FROM Account WHERE ExternalID__c = '4'");

                Assert.True(accountResult.records.FirstOrDefault().Name == newName);
            }
        }
    }
}
