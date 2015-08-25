using System;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Salesforce.Common;
using Salesforce.Common.Models;
using Salesforce.Force.FunctionalTests.Models;
using WadeWegner.Salesforce.SOAPHelpers;

namespace Salesforce.Force.FunctionalTests
{
    [TestFixture]
    public class ForceClientTests
    {
        private static readonly string SecurityToken = ConfigurationManager.AppSettings["SecurityToken"];
        private static readonly string ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
        private static readonly string ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
        private static readonly string Username = ConfigurationManager.AppSettings["Username"];
        private static readonly string Password = ConfigurationManager.AppSettings["Password"] + SecurityToken;
        private static readonly string OrganizationId = ConfigurationManager.AppSettings["OrganizationId"];

        private AuthenticationClient _auth;
        private ForceClient _client;

        [TestFixtureSetUp]
        public void Init()
        {
            _auth = new AuthenticationClient();
            _auth.UsernamePasswordAsync(ConsumerKey, ConsumerSecret, Username, Password).Wait();

            _client = new ForceClient(_auth.InstanceUrl, _auth.AccessToken, _auth.ApiVersion);
        }

        [Test]
        public async void AsyncTaskCompletion_ExpandoObject()
        {
            dynamic account = new ExpandoObject();
            account.Name = "ExpandoName" + DateTime.Now.Ticks;
            account.Description = "ExpandoDescription" + DateTime.Now.Ticks;

            var result = await _client.CreateAsync("Account", account);

            Assert.IsNotNull(result);
        }

        [Test]
        public async void UserInfo_IsNotNull()
        {
            var userInfo = await _client.UserInfo<UserInfo>(_auth.Id);

            Assert.IsNotNull(userInfo);
        }

        [Test]
        public async void Query_Accounts_Continuation()
        {
            var accounts = await _client.QueryAsync<Account>("SELECT count() FROM Account");

            if (accounts.TotalSize < 1000)
            {
                await CreateLotsOfAccounts(_client);
            }

            var contacts = await _client.QueryAsync<dynamic>("SELECT Id, Name, Description FROM Account");

            var nextRecordsUrl = contacts.NextRecordsUrl;
            var nextContacts = await _client.QueryContinuationAsync<dynamic>(nextRecordsUrl);

            Assert.IsNotNull(nextContacts);
            Assert.AreNotEqual(contacts, nextContacts);
        }

        public async Task CreateLotsOfAccounts(ForceClient forceClient)
        {
            var account = new Account { Name = "Test Account", Description = "New Account Description" };

            for (var i = 0; i < 1000; i++)
            {
                account.Name = "Test Account (" + i + ")";
                await forceClient.CreateAsync("Account", account);
            }
        }

        [Test]
        public async void Query_Count()
        {
            var accounts = await _client.QueryAsync<Account>("SELECT count() FROM Account");

            Assert.IsNotNull(accounts);
        }

        [Test]
        public async void Query_Accounts_IsNotEmpty()
        {
            var accounts = await _client.QueryAsync<Account>("SELECT Id, name, description FROM Account");

            Assert.IsNotNull(accounts);
        }

        [Test]
        public async void Query_ContactsWithAccountName_IsNotEmpty()
        {
            var queryResult = await _client.QueryAsync<Models.QueryTest.Contact>("SELECT AccountId, Account.Name, Email, Phone, Name, Title, MobilePhone FROM Contact Where Account.Name != null");

            Assert.IsNotNull(queryResult);
            Assert.IsNotNull(queryResult.Records);
            Assert.IsNotNull(queryResult.Records[0].Name);
            Assert.IsNotNull(queryResult.Records[0].Account.Name);
        }

        [Test]
        public async void Query_Accounts_BadObject()
        {
            try
            {
                await _client.QueryAsync<Account>("SELECT Id, name, description FROM BadObject");
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
            var account = new Account { Name = "New Account", Description = "New Account Description" };
            var successResponse = await _client.CreateAsync("Account", account);

            Assert.IsNotNullOrEmpty(successResponse.Id);
        }

        [Test]
        public async void QueryAll_Accounts_IsNotEmpty()
        {
            var accounts = await _client.QueryAllAsync<Account>("SELECT Id, name, description FROM Account");

            Assert.IsNotNull(accounts);
        }

        [Test]
        public async void QueryAll_Accounts_Continuation()
        {
            var accounts = await _client.QueryAllAsync<Account>("SELECT count() FROM Account");

            if (accounts.TotalSize < 1000)
            {
                await CreateLotsOfAccounts(_client);
            }

            var contacts = await _client.QueryAllAsync<dynamic>("SELECT Id, Name, Description FROM Account");

            var nextRecordsUrl = contacts.NextRecordsUrl;
            var nextContacts = await _client.QueryContinuationAsync<dynamic>(nextRecordsUrl);

            Assert.IsNotNull(nextContacts);
            Assert.AreNotEqual(contacts, nextContacts);
        }

        [Test]
        public async void Create_Contact_Typed_Annotations()
        {
            var contact = new Contact { Id = "Id", IsDeleted = false, AccountId = "AccountId", Name = "Name", FirstName = "FirstName", LastName = "LastName", Description = "Description" };
            var successResponse = await _client.CreateAsync("Contact", contact);

            Assert.IsNotNullOrEmpty(successResponse.Id);
        }

        [Test]
        public async void Create_Account_Untyped()
        {
            var account = new { Name = "New Account", Description = "New Account Description" };
            var successResponse = await _client.CreateAsync("Account", account);

            Assert.IsNotNullOrEmpty(successResponse.Id);
        }

        [Test]
        public async void Create_Account_Untyped_BadObject()
        {
            try
            {
                var account = new { Name = "New Account", Description = "New Account Description" };
                await _client.CreateAsync("BadAccount", account);
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
                var account = new { BadName = "New Account", BadDescription = "New Account Description" };
                await _client.CreateAsync("Account", account);
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
            const string originalName = "New Account";
            const string newName = "New Account 2";

            var account = new Account { Name = originalName, Description = "New Account Description" };
            var successResponse = await _client.CreateAsync("Account", account);

            account.Name = newName;

            var success = await _client.UpdateAsync("Account", successResponse.Id, account);

            Assert.IsNotNull(success);
        }

        [Test]
        public async void Update_Account_BadObject()
        {
            try
            {
                const string originalName = "New Account";
                const string newName = "New Account 2";

                var account = new Account { Name = originalName, Description = "New Account Description" };
                var successResponse = await _client.CreateAsync("Account", account);

                account.Name = newName;

                await _client.UpdateAsync("BadAccount", successResponse.Id, account);
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
                const string originalName = "New Account";
                const string newName = "New Account 2";

                var account = new { Name = originalName, Description = "New Account Description" };
                var successResponse = await _client.CreateAsync("Account", account);

                var updatedAccount = new { BadName = newName, Description = "New Account Description" };

                await _client.UpdateAsync("Account", successResponse.Id, updatedAccount);
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
            const string originalName = "New Account";
            const string newName = "New Account 2";

            var account = new Account { Name = originalName, Description = "New Account Description" };
            var successResponse = await _client.CreateAsync("Account", account);
            account.Name = newName;
            await _client.UpdateAsync("Account", successResponse.Id, account);

            var result = await _client.QueryByIdAsync<Account>("Account", successResponse.Id);

            Assert.True(result.Name == newName);
        }

        [Test]
        public async void Delete_Account_IsSuccess()
        {
            var account = new Account { Name = "New Account", Description = "New Account Description" };
            var successResponse = await _client.CreateAsync("Account", account);
            var success = await _client.DeleteAsync("Account", successResponse.Id);

            Assert.IsTrue(success);
        }

        [Test]
        public async void Delete_Account_ObjectDoesNotExist()
        {
            try
            {
                var account = new Account { Name = "New Account", Description = "New Account Description" };
                var successResponse = await _client.CreateAsync("Account", account);
                var success = await _client.DeleteAsync("BadAccount", successResponse.Id);

                Assert.IsTrue(success);
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
                const string id = "asdfasdfasdf";
                await _client.DeleteAsync("Account", id);
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
            var account = new Account { Name = "New Account", Description = "New Account Description" };
            var successResponse = await _client.CreateAsync("Account", account);
            await _client.DeleteAsync("Account", successResponse.Id);

            var result = await _client.QueryByIdAsync<Account>("Account", successResponse.Id);

            Assert.IsNull(result);
        }

        [Test]
        public async void Objects_GetAllObjects_IsNotNull()
        {
            var objects = await _client.GetObjectsAsync<object>();

            Assert.IsNotNull(objects);
        }

        [Test]
        public async void Object_BasicInformation_IsNotNull()
        {
            var accounts = await _client.BasicInformationAsync<object>("Account");

            Assert.IsNotNull(accounts);
        }

        [Test]
        public async void Object_Describe_IsNotNull()
        {
            var accounts = await _client.DescribeAsync<object>("Account");

            Assert.IsNotNull(accounts);
        }

        [Test]
        public async void Object_GetDeleted_IsNotNull()
        {
            var account = new Account { Name = "New Account to Delete", Description = "New Account Description" };
            var successResponse = await _client.CreateAsync("Account", account);

            await _client.DeleteAsync("Account", successResponse.Id);
            var dateTime = DateTime.Now;

            await Task.Run(() => Thread.Sleep(5000));

            var sdt = dateTime.Subtract(new TimeSpan(0, 0, 2, 0));
            var edt = dateTime.Add(new TimeSpan(0, 0, 2, 0));
            var deleted = await _client.GetDeleted<DeletedRecordRootObject>("Account", sdt, edt);

            Assert.IsNotNull(deleted);
            Assert.IsNotNull(deleted.DeletedRecords);
            Assert.IsTrue(deleted.DeletedRecords.Count > 0);
        }

        [Test]
        public async void Object_GetUpdated_IsNotNull()
        {
            const string originalName = "New Account";
            const string newName = "New Account 2";

            var account = new Account { Name = originalName, Description = "New Account Description" };
            var successResponse = await _client.CreateAsync("Account", account);

            account.Name = newName;
            var dateTime = DateTime.Now;

            await _client.UpdateAsync("Account", successResponse.Id, account);

            await Task.Run(() => Thread.Sleep(5000));

            var sdt = dateTime.Subtract(new TimeSpan(0, 0, 2, 0));
            var edt = dateTime.Add(new TimeSpan(0, 0, 2, 0));
            var updated = await _client.GetUpdated<UpdatedRecordRootObject>("Account", sdt, edt);

            Assert.IsNotNull(updated);
            Assert.IsTrue(updated.Ids.Count > 0);
        }

        [Test]
        public async void Object_DescribeLayout_IsNotNull()
        {
            var accountsLayout = await _client.DescribeLayoutAsync<dynamic>("Account");

            Assert.IsNotNull(accountsLayout);

            string recordTypeId = accountsLayout.recordTypeMappings[0].recordTypeId;

            Assert.IsNotNull(recordTypeId);

            var accountsLayoutForRecordTypeId = await _client.DescribeLayoutAsync<dynamic>("Account", recordTypeId);

            Assert.IsNotNull(accountsLayoutForRecordTypeId);
        }

        [Test]
        public async void Recent_IsNotNull()
        {
            var recent = await _client.RecentAsync<object>(5);

            Assert.IsNotNull(recent);
        }

        [Test]
        public async void Upsert_Account_Update_IsSuccess()
        {
            const string objectName = "Account";
            const string fieldName = "ExternalId__c";

            await CreateExternalIdField(objectName, fieldName);

            var account = new Account { Name = "Upserted Account", Description = "Upserted Account Description" };
            var success = await _client.UpsertExternalAsync(objectName, fieldName, "123", account);

            Assert.IsNotNull(success);
            Assert.IsEmpty(success.Id);
        }

        [Test]
        public async void Upsert_Account_Insert_IsSuccess()
        {
            const string objectName = "Account";
            const string fieldName = "ExternalId__c";

            await CreateExternalIdField(objectName, fieldName);

            var account = new Account { Name = "Upserted Account" + DateTime.Now.Ticks, Description = "New Upserted Account Description" + DateTime.Now.Ticks };
            var success = await _client.UpsertExternalAsync(objectName, fieldName, "123" + DateTime.Now.Ticks, account);

            Assert.IsNotNull(success);
            Assert.IsNotNull(success.Id);
            Assert.IsNotNullOrEmpty(success.Id);
        }

        [Test]
        public async void Upsert_Account_BadObject()
        {
            try
            {
                var account = new Account { Name = "New Account ExternalID", Description = "New Account Description" };
                await _client.UpsertExternalAsync("BadAccount", "ExternalID__c", "2", account);
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
                var accountBadName = new { BadName = "New Account", Description = "New Account Description" };
                await _client.UpsertExternalAsync("Account", "ExternalID__c", "3", accountBadName);
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

            const string originalName = "New Account External Upsert";
            const string newName = "New Account External Upsert 2";

            var account = new Account { Name = originalName, Description = "New Account Description" };
            await _client.UpsertExternalAsync("Account", fieldName, "4", account);

            account.Name = newName;
            await _client.UpsertExternalAsync("Account", fieldName, "4", account);

            var accountResult = await _client.QueryAsync<Account>(string.Format("SELECT Name FROM Account WHERE {0} = '4'", fieldName));
            var firstOrDefault = accountResult.Records.FirstOrDefault();

            Assert.True(firstOrDefault != null && firstOrDefault.Name == newName);
        }

        [Test]
        public async void UpdateExternalAsync_AccountSource()
        {
            dynamic a = new ExpandoObject();
            a.AccountSource = "TestAccountSource";
            a.Name = "TestAccountName";

            const string objectName = "Account";
            const string fieldName = "External_Id__c";

            await CreateExternalIdField(objectName, fieldName);

            var externalId = Convert.ToString(DateTime.Now.Ticks);

            var success = await _client.UpsertExternalAsync(objectName, fieldName, externalId, a);
            Assert.IsNotNull(success.Id);
            Assert.IsNotNull(success);

            a.AccountSource = "TestAccountSource2";

            success = await _client.UpsertExternalAsync(objectName, fieldName, externalId, a);
            Assert.IsNotNull(success);
            Assert.IsEmpty(success.Id);
        }

        [Test]
        public async void QueryLeadWithUnescapedCharactersInEmail()
        {
            const string query = "SELECT Id FROM Lead WHERE email = 'forcetoolkit+issue@gmail.com'";
            var result = await _client.QueryAsync<dynamic>(query);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Records);
            Assert.That(result.TotalSize, Is.Not.EqualTo(0));
        }

        private static async Task CreateExternalIdField(string objectName, string fieldName)
        {
            var salesforceClient = new SalesforceClient();
            var loginResult = await salesforceClient.Login(Username, Password, OrganizationId);

            await salesforceClient.CreateCustomField(objectName, fieldName, loginResult.SessionId,
                    loginResult.MetadataServerUrl, true);
        }
    }
}
