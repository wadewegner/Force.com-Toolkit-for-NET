using System.Runtime;
using ForceSDKforNET;
using ForceSDKforNet.FunctionalTests.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForceSDKforNet.FunctionalTests
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
        public async void Auth_ValidCreds_HasApiVersion()
        {
            var client = new ForceClient();

            Assert.IsNotNullOrEmpty(client.ApiVersion);
        }

        [Test]
        public async void Auth_ValidCreds_HasAccessToken()
        {
            var client = new ForceClient();
            await client.Authenticate(_consumerKey, _consumerSecret, _username, _password, _tokenRequestEndpointUrl);

            Assert.IsNotNullOrEmpty(client.AccessToken);
        }

        [Test]
        public async void Auth_ValidCreds_HasInstanceUrl()
        {
            var client = new ForceClient();
            await client.Authenticate(_consumerKey, _consumerSecret, _username, _password, _tokenRequestEndpointUrl);

            Assert.IsNotNullOrEmpty(client.InstanceUrl);
        }

        [Test]
        public async void Query_Accounts_IsNotEmpty()
        {
            var client = new ForceClient();

            await client.Authenticate(_consumerKey, _consumerSecret, _username, _password, _tokenRequestEndpointUrl);
            var accounts = await client.Query<Account>("SELECT id, name, description FROM Account");

            Assert.IsNotEmpty(accounts);
        }

        [Test]
        public async void Create_Account_Typed()
        {
            var client = new ForceClient();

            await client.Authenticate(_consumerKey, _consumerSecret, _username, _password, _tokenRequestEndpointUrl);

            var account = new Account() { Name = "New Account", Description = "New Account Description" };
            var id = await client.Create("Account", account);

            Assert.IsNotNullOrEmpty(id);
        }

        [Test]
        public async void Create_Account_Untyped()
        {
            var client = new ForceClient();

            await client.Authenticate(_consumerKey, _consumerSecret, _username, _password, _tokenRequestEndpointUrl);

            var account = new { Name = "New Account", Description = "New Account Description" };
            var id = await client.Create("Account", account);

            Assert.IsNotNullOrEmpty(id);
        }

        [Test]
        public async void Update_Account_IsSuccess()
        {
            var client = new ForceClient();

            await client.Authenticate(_consumerKey, _consumerSecret, _username, _password, _tokenRequestEndpointUrl);

            string originalName = "New Account";
            string newName = "New Account 2";

            var account = new Account() { Name = originalName, Description = "New Account Description" };
            var id = await client.Create("Account", account);

            account.Name = newName;

            var success = await client.Update("Account", id, account);

            Assert.IsTrue(success);
        }

        //[Test]
        //public async void Update_Account_NameChanged()
        //{
        //    var client = new ForceClient();

        //    await client.Authenticate(_consumerKey, _consumerSecret, _username, _password, _tokenRequestEndpointUrl);

        //    string originalName = "New Account";
        //    string newName = "New Account 2";

        //    var account = new Account() { Name = originalName, Description = "New Account Description" };
        //    var id = await client.Create("Account", account);

        //    account.Name = newName;

        //    var success = await client.Update("Account", id, account);

        //    if (success)
        //    {
        //        // query by id
        //        Assert.True(false);
        //    }

        //    Assert.IsTrue(success);
        //}


        [Test]
        public async void Delete_Account_IsSuccess()
        {
            var client = new ForceClient();

            await client.Authenticate(_consumerKey, _consumerSecret, _username, _password, _tokenRequestEndpointUrl);

            var account = new Account() { Name = "New Account", Description = "New Account Description" };
            var id = await client.Create("Account", account);
            var success = await client.Delete("Account", id);

            Assert.IsTrue(success);
        }

        //[Test]
        //public async void Delete_Account_ValidateIsGone()
        //{
        //    Assert.True(false);
        //}

    }
}
