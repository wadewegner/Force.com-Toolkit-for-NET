//TODO: add license header

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using NUnit.Framework;
using Salesforce.Common;
using Salesforce.Common.Models;
using Salesforce.Force.FunctionalTests.Models;
//using WadeWegner.Salesforce.SOAPHelpers;
//using WadeWegner.Salesforce.SOAPHelpers.Models;
using System.Diagnostics;
using WadeWegner.Salesforce.SOAPHelpers;

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
        private static string _organizationId = ConfigurationSettings.AppSettings["OrganizationId"];
#pragma warning restore 618

        private AuthenticationClient _auth;

        public async Task<ForceClient> GetForceClient(HttpClient httpClient)
        {
            _auth = new AuthenticationClient(httpClient);
            await _auth.UsernamePasswordAsync(_consumerKey, _consumerSecret, _username, _password);

            var client = new ForceClient(_auth.InstanceUrl, _auth.AccessToken, _auth.ApiVersion, httpClient);
            return client;
        }

        [Test]
        public async void UserInfo_IsNotNull()
        {
            using (var httpClient = new HttpClient())
            {
                var client = await GetForceClient(httpClient);
                var userInfo = await client.UserInfo<dynamic>(_auth.AccessToken, _auth.Id);

                Assert.IsNotNull(userInfo);
            }
        }

        [Test]
        public async void Query_Accounts_Continuation()
        {
            using (var httpClient = new HttpClient())
            {
                var client = await GetForceClient(httpClient);
                var accounts = await client.QueryAsync<Account>("SELECT count() FROM Account");

                if (accounts.totalSize < 1000)
                {
                    await CreateLotsOfAccounts(client);
                }

                var contacts = await client.QueryAsync<dynamic>("SELECT Id, Name, Description FROM Account");

                var nextRecordsUrl = contacts.nextRecordsUrl;
                var nextContacts = await client.QueryContinuationAsync<dynamic>(nextRecordsUrl);

                Assert.IsNotNull(nextContacts);
                Assert.AreNotEqual(contacts, nextContacts);
            }
        }

        public async Task CreateLotsOfAccounts(ForceClient client)
        {
            var account = new Account { Name = "Test Account", Description = "New Account Description" };

            for (var i = 0; i < 1000; i++)
            {
                account.Name = "Test Account (" + i + ")";
                await client.CreateAsync("Account", account);
            }
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
        public async void Object_BasicInformation_IsNotNull()
        {
            using (var httpClient = new HttpClient())
            {
                var client = await GetForceClient(httpClient);
                var accounts = await client.GetBasicInformationAsync<object>("Account");

                Assert.IsNotNull(accounts);
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

        //TODO: Fix WadeWegner nuget and readd
        //[Test]
        //public async void Upsert_Account_IsSuccess()
        //{
        //    const string objectName = "Account";
        //    const string fieldName = "ExternalId__c";

        //    await CreateExternalIdField(objectName, fieldName);

        //    using (var httpClient = new HttpClient())
        //    {
        //        var client = await GetForceClient(httpClient);
        //        var account = new Account { Name = "Upserted Account", Description = "New Upserted Account Description" };
        //        var success = await client.UpsertExternalAsync(objectName, fieldName, "123", account);

        //        Assert.IsTrue(success);
        //    }
        //}

        [Test]
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
        public async void Upsert_Account_NameChanged()
        {
            const string fieldName = "ExternalId__c";
            await CreateExternalIdField("Account", fieldName);

            using (var httpClient = new HttpClient())
            {
                var client = await GetForceClient(httpClient);

                var originalName = "New Account External Upsert";
                var newName = "New Account External Upsert 2";

                var account = new Account { Name = originalName, Description = "New Account Description" };
                await client.UpsertExternalAsync("Account", fieldName, "4", account);

                account.Name = newName;
                await client.UpsertExternalAsync("Account", fieldName, "4", account);

                var accountResult = await client.QueryAsync<Account>(string.Format("SELECT Name FROM Account WHERE {0} = '4'", fieldName));
                var firstOrDefault = accountResult.records.FirstOrDefault();

                Assert.True(firstOrDefault != null && firstOrDefault.Name == newName);
            }
        }

        private static async Task CreateExternalIdField(string objectName, string fieldName)
        {
            var salesforceClient = new SalesforceClient();
            var loginResult = await salesforceClient.Login(_username, _password, _organizationId);

            await salesforceClient.CreateCustomField(objectName, fieldName, loginResult.SessionId,
                    loginResult.MetadataServerUrl, true);
        }
    }
}
