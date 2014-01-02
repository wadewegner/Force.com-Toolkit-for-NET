using System.Runtime;
using ForceToolkitForNET;
using ForceToolkitForNET.FunctionalTests.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonToolkitForNET;

namespace ForceToolkitForNET.FunctionalTests
{
    [TestFixture]
    public class ForceClientTests
    {
        private static string _tokenRequestEndpointUrl = ConfigurationSettings.AppSettings["TokenRequestEndpointUrl"];
        private static string _securityToken = ConfigurationSettings.AppSettings["SecurityToken"];
        private static string _consumerKey = ConfigurationSettings.AppSettings["ConsumerKey"];
        private static string _consumerSecret = ConfigurationSettings.AppSettings["ConsumerSecret"];
        private static string _username = ConfigurationSettings.AppSettings["Username"];
        private static string _password = ConfigurationSettings.AppSettings["Password"] + _securityToken;

        [Test]
        public async void Query_Accounts_IsNotEmpty()
        {
            var auth = new AuthClient();
            await auth.Authenticate(_consumerKey, _consumerSecret, _username, _password, _tokenRequestEndpointUrl);

            var client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);

            var accounts = await client.Query<Account>("SELECT id, name, description FROM Account");

            Assert.IsNotEmpty(accounts);
        }

        [Test]
        public async void Create_Account_Typed()
        {
            var auth = new AuthClient();
            await auth.Authenticate(_consumerKey, _consumerSecret, _username, _password, _tokenRequestEndpointUrl);

            var client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);

            var account = new Account() { Name = "New Account", Description = "New Account Description" };
            var id = await client.Create("Account", account);

            Assert.IsNotNullOrEmpty(id);
        }

        [Test]
        public async void Create_Account_Untyped()
        {
            var auth = new AuthClient();
            await auth.Authenticate(_consumerKey, _consumerSecret, _username, _password, _tokenRequestEndpointUrl);

            var client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);

            var account = new { Name = "New Account", Description = "New Account Description" };
            var id = await client.Create("Account", account);

            Assert.IsNotNullOrEmpty(id);
        }

        [Test]
        public async void Update_Account_IsSuccess()
        {
            var auth = new AuthClient();
            await auth.Authenticate(_consumerKey, _consumerSecret, _username, _password, _tokenRequestEndpointUrl);

            var client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);

            string originalName = "New Account";
            string newName = "New Account 2";

            var account = new Account() { Name = originalName, Description = "New Account Description" };
            var id = await client.Create("Account", account);

            account.Name = newName;

            var success = await client.Update("Account", id, account);

            Assert.IsTrue(success);
        }

        [Test]
        public async void Update_Account_NameChanged()
        {
            var auth = new AuthClient();
            await auth.Authenticate(_consumerKey, _consumerSecret, _username, _password, _tokenRequestEndpointUrl);

            var client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);

            var originalName = "New Account";
            var newName = "New Account 2";

            var account = new Account() { Name = originalName, Description = "New Account Description" };
            var id = await client.Create("Account", account);

            account.Name = newName;

            await client.Update("Account", id, account);

            var result = await client.QueryById<Account>("Account", id);

            Assert.True(result.Name == newName);
        }


        [Test]
        public async void Delete_Account_IsSuccess()
        {
            var auth = new AuthClient();
            await auth.Authenticate(_consumerKey, _consumerSecret, _username, _password, _tokenRequestEndpointUrl);

            var client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);

            var account = new Account() { Name = "New Account", Description = "New Account Description" };
            var id = await client.Create("Account", account);
            var success = await client.Delete("Account", id);

            Assert.IsTrue(success);
        }

        [Test]
        public async void Delete_Account_ValidateIsGone()
        {
            var auth = new AuthClient();
            await auth.Authenticate(_consumerKey, _consumerSecret, _username, _password, _tokenRequestEndpointUrl);

            var client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);

            var account = new Account() { Name = "New Account", Description = "New Account Description" };
            var id = await client.Create("Account", account);
            await client.Delete("Account", id);

            var result = await client.QueryById<Account>("Account", id);

            Assert.IsNull(result);
        }

        [Test]
        public async void Objects_Get_IsNotNull()
        {
            var auth = new AuthClient();
            await auth.Authenticate(_consumerKey, _consumerSecret, _username, _password, _tokenRequestEndpointUrl);

            var client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);

            var objects = await client.GetObjects();

            Assert.IsNotNull(objects);
        }

        [Test]
        public async void Object_Describe_IsNotNull()
        {
            var auth = new AuthClient();
            await auth.Authenticate(_consumerKey, _consumerSecret, _username, _password, _tokenRequestEndpointUrl);

            var client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);

            var accounts = await client.Describe<object>("Account");

            Assert.IsNotNull(accounts);
        }
    }
}
