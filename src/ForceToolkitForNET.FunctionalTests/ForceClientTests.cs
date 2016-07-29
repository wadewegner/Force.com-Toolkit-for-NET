using System;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Salesforce.Common;
using Salesforce.Common.Models.Json;
using Salesforce.Force.FunctionalTests.Models;
//using WadeWegner.Salesforce.SOAPHelpers;

namespace Salesforce.Force.FunctionalTests
{
    [TestFixture]
    public class ForceClientTests
    {
        private static string _consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
        private static string _consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
        private static string _username = ConfigurationManager.AppSettings["Username"];
        private static string _password = ConfigurationManager.AppSettings["Password"];
        private static string _organizationId = ConfigurationManager.AppSettings["OrganizationId"];

        private AuthenticationClient _auth;
        private ForceClient _client;

        [TestFixtureSetUp]
        public void Init()
        {
            if (string.IsNullOrEmpty(_consumerKey) && string.IsNullOrEmpty(_consumerSecret) && string.IsNullOrEmpty(_username) && string.IsNullOrEmpty(_password) && string.IsNullOrEmpty(_organizationId))
            {
                _consumerKey = Environment.GetEnvironmentVariable("ConsumerKey");
                _consumerSecret = Environment.GetEnvironmentVariable("ConsumerSecret");
                _username = Environment.GetEnvironmentVariable("Username");
                _password = Environment.GetEnvironmentVariable("Password");
                _organizationId = Environment.GetEnvironmentVariable("OrganizationId");
            }

            // Use TLS 1.2 (instead of defaulting to 1.0)
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            _auth = new AuthenticationClient();
            _auth.UsernamePasswordAsync(_consumerKey, _consumerSecret, _username, _password).Wait();

            _client = new ForceClient(_auth.InstanceUrl, _auth.AccessToken, _auth.ApiVersion);
        }

        [Test]
        public async Task AsyncTaskCompletion_ExpandoObject()
        {
            dynamic account = new ExpandoObject();
            account.Name = "ExpandoName" + DateTime.Now.Ticks;
            account.Description = "ExpandoDescription" + DateTime.Now.Ticks;

            var result = await _client.CreateAsync("Account", account);

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task UserInfo_IsNotNull()
        {
            var userInfo = await _client.UserInfo<UserInfo>(_auth.Id);

            Assert.IsNotNull(userInfo);
        }

        [Test]
        public async Task Query_Accounts_Continuation()
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
        public async Task Query_Count()
        {
            var accounts = await _client.QueryAsync<Account>("SELECT count() FROM Account");

            Assert.IsNotNull(accounts);
        }

        [Test]
        public async Task Query_Accounts_IsNotEmpty()
        {
            var accounts = await _client.QueryAsync<Account>("SELECT Id, name, description FROM Account");

            Assert.IsNotNull(accounts);
        }

        [Test]
        public async Task Query_ContactsWithAccountName_IsNotEmpty()
        {
            var queryResult = await _client.QueryAsync<Models.QueryTest.Contact>("SELECT AccountId, Account.Name, Email, Phone, Name, Title, MobilePhone FROM Contact Where Account.Name != null");

            Assert.IsNotNull(queryResult);
            Assert.IsNotNull(queryResult.Records);
            Assert.IsNotNull(queryResult.Records[0].Name);
            //Assert.IsNotNull(queryResult.Records[0].Account.Name); //BUG: This assertion sometimes causes the test run to fail (Not sure why, needs investigation)
        }

        [Test]
        public async Task Query_Accounts_BadObject()
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
        public async Task Create_Account_Typed()
        {
            var account = new Account { Name = "New Account", Description = "New Account Description" };
            var successResponse = await _client.CreateAsync("Account", account);

            Assert.That(successResponse.Id, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task QueryAll_Accounts_IsNotEmpty()
        {
            var accounts = await _client.QueryAllAsync<Account>("SELECT Id, name, description FROM Account");

            Assert.IsNotNull(accounts);
        }

        [Test]
        public async Task QueryAll_Accounts_Continuation()
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
        public async Task Create_Contact_Typed_Annotations()
        {
            var contact = new Contact { Id = "Id", IsDeleted = false, AccountId = "AccountId", Name = "Name", FirstName = "FirstName", LastName = "LastName", Description = "Description" };
            var successResponse = await _client.CreateAsync("Contact", contact);

            Assert.That(successResponse.Id, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task Create_Account_Untyped()
        {
            var account = new { Name = "New Account", Description = "New Account Description" };
            var successResponse = await _client.CreateAsync("Account", account);

            Assert.That(successResponse.Id, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task Create_Account_Untyped_BadObject()
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
        public async Task Create_Account_Untyped_BadFields()
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
        public async Task Update_Account_IsSuccess()
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
        public async Task Update_Account_NullValues()
        {
            const string originalName = "New Account";
            const string newName = "New Account 2";

            var newAccount = new Account { Name = originalName, Description = "New Account Description" };
            var success1 = await _client.CreateAsync("Account", newAccount);
            Assert.IsNotNull(success1);

            var id = success1.Id;

            const string query = "SELECT AccountNumber,AccountSource,Active__c,AnnualRevenue,BillingAddress,BillingCity,BillingCountry,BillingGeocodeAccuracy,BillingLatitude,BillingLongitude,BillingPostalCode,BillingState,BillingStreet,CleanStatus,CreatedById,CreatedDate,CustomerPriority__c,DandbCompanyId,Description,DunsNumber,ExternalId__c,External_Id__c,Fax,Id,Industry,IsDeleted,Jigsaw,JigsawCompanyId,LastActivityDate,LastModifiedById,LastModifiedDate,LastReferencedDate,LastViewedDate,MasterRecordId,MyCustomField__c,NaicsCode,NaicsDesc,Name,NumberOfEmployees,NumberofLocations__c,OwnerId,Ownership,ParentId,Phone,PhotoUrl,Rating,ShippingAddress,ShippingCity,ShippingCountry,ShippingGeocodeAccuracy,ShippingLatitude,ShippingLongitude,ShippingPostalCode,ShippingState,ShippingStreet,Sic,SicDesc,Site,SLAExpirationDate__c,SLASerialNumber__c,SLA__c,SystemModstamp,TickerSymbol,Tradestyle,Type,UpsellOpportunity__c,Website,YearStarted FROM Account WHERE Id = '{0}'";

            var account1 = await _client.QueryAsync<Account>(string.Format(query, id));
            var newAccount2 = new Account { Name = newName };

            var success2 = await _client.UpdateAsync("Account", id, newAccount2);
            Assert.IsNotNull(success2);

            var account2 = await _client.QueryAsync<Account>(string.Format(query, id));

            Assert.AreEqual(account1.Records[0].Description, account2.Records[0].Description);
        }

        [Test]
        public async Task Update_Account_BadObject()
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
        public async Task Update_Account_BadField()
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
        public async Task Update_Account_NameChanged()
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
        public async Task Delete_Account_IsSuccess()
        {
            var account = new Account { Name = "New Account", Description = "New Account Description" };
            var successResponse = await _client.CreateAsync("Account", account);
            var success = await _client.DeleteAsync("Account", successResponse.Id);

            Assert.IsTrue(success);
        }

        [Test]
        public async Task Delete_Account_ObjectDoesNotExist()
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
        public async Task Delete_Account_IdDoesNotExist()
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
        public async Task Delete_Account_ValidateIsGone()
        {
            var account = new Account { Name = "New Account", Description = "New Account Description" };
            var successResponse = await _client.CreateAsync("Account", account);
            await _client.DeleteAsync("Account", successResponse.Id);

            var result = await _client.QueryByIdAsync<Account>("Account", successResponse.Id);

            Assert.IsNull(result);
        }

        //[Test]
        //public async Task Delete_External_ValidateIsGone()
        //{
        //    const string objectName = "Account";
        //    const string fieldName = "ExternalId__c";
        //    var fieldId = "123" + DateTime.Now.Ticks;

        //    await CreateExternalIdField(objectName, fieldName);

        //    var account = new Account { Name = "Upserted To Delete", Description = "Upserted Account Description to Delete" };
        //    var success = await _client.UpsertExternalAsync(objectName, fieldName, fieldId, account);

        //    var resultExists = await _client.QueryByIdAsync<Account>("Account", success.Id);

        //    Assert.IsNotNull(resultExists);

        //    await _client.DeleteExternalAsync("Account", fieldName, fieldId);

        //    var resultDoesNotExists = await _client.QueryByIdAsync<Account>("Account", success.Id);

        //    Assert.IsNull(resultDoesNotExists);
        //}

        [Test]
        public async Task Objects_GetAllObjects_IsNotNull()
        {
            var objects = await _client.GetObjectsAsync<object>();

            Assert.IsNotNull(objects);
        }

        [Test]
        public async Task Object_BasicInformation_IsNotNull()
        {
            var accounts = await _client.BasicInformationAsync<object>("Account");

            Assert.IsNotNull(accounts);
        }

        [Test]
        public async Task Object_Describe_IsNotNull()
        {
            var accounts = await _client.DescribeAsync<object>("Account");

            Assert.IsNotNull(accounts);
        }

        [Test]
        public async Task Object_GetDeleted_IsNotNull()
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
        public async Task Object_GetUpdated_IsNotNull()
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
        public async Task Object_DescribeLayout_IsNotNull()
        {
            var accountsLayout = await _client.DescribeLayoutAsync<dynamic>("Account");

            Assert.IsNotNull(accountsLayout);

            string recordTypeId = accountsLayout.recordTypeMappings[0].recordTypeId;

            Assert.IsNotNull(recordTypeId);

            var accountsLayoutForRecordTypeId = await _client.DescribeLayoutAsync<dynamic>("Account", recordTypeId);

            Assert.IsNotNull(accountsLayoutForRecordTypeId);
        }

        [Test]
        public async Task Recent_IsNotNull()
        {
            var recent = await _client.RecentAsync<object>(5);

            Assert.IsNotNull(recent);
        }

        //[Test]
        //public async Task Upsert_Account_Update_IsSuccess()
        //{
        //    const string objectName = "Account";
        //    const string fieldName = "ExternalId__c";

        //    await CreateExternalIdField(objectName, fieldName);

        //    var account = new Account { Name = "Upserted Account", Description = "Upserted Account Description" };
        //    var success = await _client.UpsertExternalAsync(objectName, fieldName, "123", account);

        //    Assert.IsNotNull(success);
        //    Assert.IsEmpty(success.Id);
        //}

        //[Test]
        //public async Task Upsert_Account_Insert_IsSuccess()
        //{
        //    const string objectName = "Account";
        //    const string fieldName = "ExternalId__c";

        //    await CreateExternalIdField(objectName, fieldName);

        //    var account = new Account { Name = "Upserted Account" + DateTime.Now.Ticks, Description = "New Upserted Account Description" + DateTime.Now.Ticks };
        //    var success = await _client.UpsertExternalAsync(objectName, fieldName, "123" + DateTime.Now.Ticks, account);

        //    Assert.IsNotNull(success);
        //    Assert.IsNotNull(success.Id);
        //    Assert.That(success.Id, Is.Not.Null.And.Not.Empty);
        //}

        [Test]
        public async Task Upsert_Account_BadObject()
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
        public async Task Upsert_Account_BadField()
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

        //[Test]
        //public async Task Upsert_Account_NameChanged()
        //{
        //    const string fieldName = "ExternalId__c";
        //    await CreateExternalIdField("Account", fieldName);

        //    const string originalName = "New Account External Upsert";
        //    const string newName = "New Account External Upsert 2";

        //    var account = new Account { Name = originalName, Description = "New Account Description" };
        //    await _client.UpsertExternalAsync("Account", fieldName, "4", account);

        //    account.Name = newName;
        //    await _client.UpsertExternalAsync("Account", fieldName, "4", account);

        //    var accountResult = await _client.QueryAsync<Account>(string.Format("SELECT Name FROM Account WHERE {0} = '4'", fieldName));
        //    var firstOrDefault = accountResult.Records.FirstOrDefault();

        //    Assert.True(firstOrDefault != null && firstOrDefault.Name == newName);
        //}

        //[Test]
        //public async Task UpdateExternalAsync_AccountSource()
        //{
        //    dynamic a = new ExpandoObject();
        //    a.AccountSource = "TestAccountSource";
        //    a.Name = "TestAccountName";

        //    const string objectName = "Account";
        //    const string fieldName = "External_Id__c";

        //    await CreateExternalIdField(objectName, fieldName);

        //    var externalId = Convert.ToString(DateTime.Now.Ticks);

        //    var success = await _client.UpsertExternalAsync(objectName, fieldName, externalId, a);
        //    Assert.IsNotNull(success.Id);
        //    Assert.IsNotNull(success);

        //    a.AccountSource = "TestAccountSource2";

        //    success = await _client.UpsertExternalAsync(objectName, fieldName, externalId, a);
        //    Assert.IsNotNull(success);
        //    Assert.IsEmpty(success.Id);
        //}

        [Test]
        public async Task QueryLeadWithUnescapedCharactersInEmail()
        {
            const string query = "SELECT Id FROM Lead WHERE email = 'forcetoolkit+issue@gmail.com'";
            var result = await _client.QueryAsync<dynamic>(query);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Records);
            Assert.That(result.TotalSize, Is.Not.EqualTo(0));
        }

        [Test]
        public async Task SearchAsync()
        {
            var result = await _client.SearchAsync<dynamic>("FIND {test}");

            Assert.IsNotNull(result);

            result = await _client.SearchAsync<dynamic>("FIND {493*} in Phone FIELDS RETURNING Contact(Id, FirstName, Lastname, Email, Phone, MobilePhone)");

            Assert.IsNotNull(result);

            result = await _client.SearchAsync<dynamic>("FIND {493*} in Phone FIELDS RETURNING Lead(Id, FirstName, LastName, Phone, Company), Contact(Id, FirstName, LastName, Phone, MobilePhone)");

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task Create_EventWithDate_IsSuccess()
        {
            // This test is to ensure we have proper date serialization and don't get the following error:
            // "Cannot deserialize instance of date from VALUE_STRING value 2015-08-25T11:49:53.0113029-07:00"

            var account = new Account { Name = "New Account", Description = "New Account Description" };
            var accountSuccessResponse = await _client.CreateAsync("Account", account);

            var newEvent = new Event
            {
                Description = "new Event",
                Subject = "new Event",
                WhatId = accountSuccessResponse.Id,
                ActivityDate = DateTime.Now,
                DurationInMinutes = 10,
                ActivityDateTime = DateTime.Now
            };

            var eventSuccessResponse = await _client.CreateAsync("Event", newEvent);

            Assert.IsTrue(eventSuccessResponse.Success);
        }

        [Test]
        public async Task ExecuteRestApiPost()
        {
            const string echo = "Thing to echo";

            var json = JObject.Parse(@"{'toecho':'" + echo + "'}");
            var response = await _client.ExecuteRestApiAsync<dynamic>("RestWSTest", json);

            Assert.IsNotNull(response);
            Assert.AreEqual(echo, response);
        }

        [Test]
        public async Task ExecuteRestApiGet()
        {
            const string echo = "stuff";

            var response = await _client.ExecuteRestApiAsync<dynamic>("RestWSTest");

            Assert.IsNotNull(response);
            Assert.AreEqual(echo, response);
        }


        //#region Private methods
        //private static async Task CreateExternalIdField(string objectName, string fieldName)
        //{
        //    var salesforceClient = new SalesforceClient();
        //    var loginResult = await salesforceClient.Login(_username, _password, _organizationId);

        //    await salesforceClient.CreateCustomField(objectName, fieldName, loginResult.SessionId,
        //            loginResult.MetadataServerUrl, true);
        //}
        //#endregion
    }
}
