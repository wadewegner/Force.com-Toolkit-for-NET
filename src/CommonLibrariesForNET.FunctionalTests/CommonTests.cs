using System;
using System.Configuration;
using System.Net.Http;
using NUnit.Framework;
using Salesforce.Common.FunctionalTests.Models;
using Salesforce.Common.Models.Json;

namespace Salesforce.Common.FunctionalTests
{
	[TestFixture]
    public class CommonTests
    {
        private static readonly string TokenRequestEndpointUrl = ConfigurationManager.AppSettings["TokenRequestEndpointUrl"];
        private static readonly string SecurityToken = ConfigurationManager.AppSettings["SecurityToken"];
        private static readonly string ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
        private static readonly string ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
        private static readonly string Username = ConfigurationManager.AppSettings["Username"];
        private static readonly string Password = ConfigurationManager.AppSettings["Password"] + SecurityToken;
	    
	    private AuthenticationClient _auth;
	    private ServiceHttpClient _serviceHttpClient;

	    [TestFixtureSetUp]
        public void Init()
        {
            _auth = new AuthenticationClient();
            _auth.UsernamePasswordAsync(ConsumerKey, ConsumerSecret, Username, Password, TokenRequestEndpointUrl).Wait();

            _serviceHttpClient = new ServiceHttpClient(_auth.InstanceUrl, _auth.ApiVersion, _auth.AccessToken, new HttpClient());
        }

        //[Test]
        //public async void Get_UserInfo()
        //{
        //    var objectName = new FormUrlEncodedContent(new[]
        //        {
        //            new KeyValuePair<string, string>("AccessToken", _auth.AccessToken)
        //        });

        //    var response = await _serviceHttpClient.HttpGetAsync<UserInfo>(new Uri(_auth.Id));

        //    Assert.IsNotNull(response);
        //}

        [Test]
        public async void Query_Describe()
        {
            const string objectName = "Account";
            var response = await _serviceHttpClient.HttpGetAsync<dynamic>(string.Format("sobjects/{0}", objectName));
            
            Assert.IsNotNull(response);
        }

        [Test]
        public async void Query_Objects()
        {
            var response = await _serviceHttpClient.HttpGetAsync<DescribeGlobalResult<dynamic>>(string.Format("sobjects"));

            Assert.IsTrue(response.MaxBatchSize > 0);
            Assert.IsTrue(response.SObjects.Count > 0);
        }

        [Test]
        public async void Query_Select_Account()
        {
            const string query = "SELECT Id FROM Account";
            var response = await _serviceHttpClient.HttpGetAsync<QueryResult<dynamic>>(string.Format("query?q={0}", query));

            Assert.IsTrue(response.TotalSize > 0);
            Assert.IsTrue(response.Records.Count > 0);
        }

        [Test]
        public async void Query_Select_Count()
        {
            const string query = "SELECT count() FROM Account";
            var response = await _serviceHttpClient.HttpGetAsync<QueryResult<dynamic>>(string.Format("query?q={0}", query));

            Assert.IsTrue(response.TotalSize > 0);
            Assert.IsTrue(response.Records.Count == 0);
        }

        [Test]
        public void Auth_UsernamePassword_HasAccessToken()
        {
            Assert.IsNotNullOrEmpty(_auth.AccessToken);
        }

        [Test]
        public void Auth_UsernamePassword_HasInstanceUrl()
        {
            Assert.IsNotNullOrEmpty(_auth.InstanceUrl);
        }

        [Test]
        public async void Auth_InvalidLogin()
        {
            try
            {
                await _auth.UsernamePasswordAsync(ConsumerKey, ConsumerSecret, Username, "WRONGPASSWORD", TokenRequestEndpointUrl);
            }
            catch (ForceAuthException ex)
            {
                Assert.IsNotNull(ex);
                Assert.IsNotNull(ex.Message);
                Assert.IsNotNull(ex.Error);
            }
        }

	    [Test]
	    public async void Upsert_Update_CheckReturn()
	    {
            var account = new Account { Name = "New Account ExternalID", Description = "New Account Description" };
            var response = await _serviceHttpClient.HttpPatchAsync(account, string.Format("sobjects/{0}/{1}/{2}", "Account", "ExternalID__c", "2"));

            Assert.IsNotNull(response);
	    }

        [Test]
        public async void Upsert_New_CheckReturnInclude()
        {
            var account = new Account { Name = "New Account" + DateTime.Now.Ticks, Description = "New Account Description" + DateTime.Now.Ticks };
            var response = await _serviceHttpClient.HttpPatchAsync(account, string.Format("sobjects/{0}/{1}/{2}", "Account", "ExternalID__c", DateTime.Now.Ticks));

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Id);
        }

	    [Test]
	    public async void BadTokenHandling()
	    {
            _auth = new AuthenticationClient();
            _auth.UsernamePasswordAsync(ConsumerKey, ConsumerSecret, Username, Password, TokenRequestEndpointUrl).Wait();

	        var badToken = "badtoken";
            var serviceHttpClient = new ServiceHttpClient(_auth.InstanceUrl, _auth.ApiVersion, badToken, new HttpClient());

            const string query = "SELECT count() FROM Account";

	        try
	        {
                await serviceHttpClient.HttpGetAsync<QueryResult<dynamic>>(string.Format("query?q={0}", query));
	        }
            catch (ForceException ex)
            {
                Assert.IsNotNull(ex);
                Assert.IsNotNull(ex.Message);
                Assert.That(ex.Message, Is.EqualTo("Session expired or invalid"));
                Assert.IsNotNull(ex.Error);
            }
	    }
    }
}
