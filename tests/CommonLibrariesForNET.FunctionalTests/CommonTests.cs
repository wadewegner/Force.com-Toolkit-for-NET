using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Salesforce.Common.FunctionalTests.Models;
using Salesforce.Common.Models.Json;

namespace Salesforce.Common.FunctionalTests
{
	[TestFixture]
    public class CommonTests
    {
        private static string _consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
        private static string _consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
        private static string _username = ConfigurationManager.AppSettings["Username"];
        private static string _password = ConfigurationManager.AppSettings["Password"];
	    
	    private AuthenticationClient _auth;
	    private JsonHttpClient _jsonHttpClient;

        [OneTimeSetUp]
        public void Init()
        {
            if (string.IsNullOrEmpty(_consumerKey) && string.IsNullOrEmpty(_consumerSecret) && string.IsNullOrEmpty(_username) && string.IsNullOrEmpty(_password))
            {
                _consumerKey = Environment.GetEnvironmentVariable("ConsumerKey");
                _consumerSecret = Environment.GetEnvironmentVariable("ConsumerSecret");
                _username = Environment.GetEnvironmentVariable("Username");
                _password = Environment.GetEnvironmentVariable("Password");
            }

            // Use TLS 1.2 (instead of defaulting to 1.0)

            const int SecurityProtocolTypeTls11 = 768;
            const int SecurityProtocolTypeTls12 = 3072;

            ServicePointManager.SecurityProtocol |= (SecurityProtocolType)(SecurityProtocolTypeTls12 | SecurityProtocolTypeTls11); 

            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            _auth = new AuthenticationClient();
            _auth.UsernamePasswordAsync(_consumerKey, _consumerSecret, _username, _password).Wait();
            
            _jsonHttpClient = new JsonHttpClient(_auth.InstanceUrl, _auth.ApiVersion, _auth.AccessToken, new HttpClient());
        }

        [Test]
        public async Task Get_UserInfo()
        {
            var objectName = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("AccessToken", _auth.AccessToken)
                });

            var response = await _jsonHttpClient.HttpGetAsync<UserInfo>(new Uri(_auth.Id));

            Assert.IsNotNull(response);
        }

        [Test]
        public async Task Query_Describe()
        {
            const string objectName = "Account";
            var response = await _jsonHttpClient.HttpGetAsync<dynamic>(string.Format("sobjects/{0}", objectName));
            
            Assert.IsNotNull(response);
        }

        [Test]
        public async Task Query_Objects()
        {
            var response = await _jsonHttpClient.HttpGetAsync<DescribeGlobalResult<dynamic>>(string.Format("sobjects"));

            Assert.IsTrue(response.MaxBatchSize > 0);
            Assert.IsTrue(response.SObjects.Count > 0);
        }

        [Test]
        public async Task Query_Select_Account()
        {
            const string query = "SELECT Id FROM Account";
            var response = await _jsonHttpClient.HttpGetAsync<QueryResult<dynamic>>(string.Format("query?q={0}", query));

            Assert.IsTrue(response.TotalSize > 0);
            Assert.IsTrue(response.Records.Count > 0);
        }

        [Test]
        public async Task Query_Select_Count()
        {
            const string query = "SELECT count() FROM Account";
            var response = await _jsonHttpClient.HttpGetAsync<QueryResult<dynamic>>(string.Format("query?q={0}", query));

            Assert.IsTrue(response.TotalSize > 0);
            Assert.IsTrue(response.Records.Count == 0);
        }

        [Test]
        public void Auth_UsernamePassword_HasAccessToken()
        {
            Assert.That(_auth.AccessToken, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void Auth_UsernamePassword_HasInstanceUrl()
        {
            Assert.That(_auth.InstanceUrl, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task Auth_InvalidLogin()
        {
            try
            {
                await _auth.UsernamePasswordAsync(_consumerKey, _consumerSecret, _username, "WRONGPASSWORD");
            }
            catch (ForceAuthException ex)
            {
                Assert.IsNotNull(ex);
                Assert.IsNotNull(ex.Message);
                Assert.IsNotNull(ex.Error);
                Assert.IsNotNull(ex.HttpStatusCode);

                Assert.AreEqual(ex.Message, "authentication failure");
                Assert.AreEqual(ex.Error, Error.InvalidGrant);
                Assert.AreEqual(ex.HttpStatusCode, HttpStatusCode.BadRequest);
            }
        }

	    [Test]
	    public async Task Auth_InvalidRequestEndpointUrl()
	    {
	        const string requestEndpointUrl = "https://login.salesforce.com/services/oauth2/authorizeee"; // typo in the url

	        try
	        {
                await _auth.WebServerAsync("clientId", "clientSecret", "sfdc://success", "code", requestEndpointUrl);
	        }
            catch (ForceAuthException ex)
	        {
	            Assert.IsNotNull(ex);
                
                Assert.AreEqual(ex.Error, Error.UnknownException);
                Assert.AreEqual(ex.Message, "Unexpected character encountered while parsing value: <. Path '', line 0, position 0.");
	        }
	    }

	    [Test]
	    public async Task Upsert_Update_CheckReturn()
	    {
            var account = new Account { Name = "New Account ExternalID", Description = "New Account Description" };
            var response = await _jsonHttpClient.HttpPatchAsync(account, string.Format("sobjects/{0}/{1}/{2}", "Account", "ExternalID__c", "2"));

            Assert.IsNotNull(response);
	    }

        [Test]
        public async Task Upsert_New_CheckReturnInclude()
        {
            var account = new Account { Name = "New Account" + DateTime.Now.Ticks, Description = "New Account Description" + DateTime.Now.Ticks };
            var response = await _jsonHttpClient.HttpPatchAsync(account, string.Format("sobjects/{0}/{1}/{2}", "Account", "ExternalID__c", DateTime.Now.Ticks));

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Id);
        }

	    [Test]
	    public async Task BadTokenHandling()
	    {
	        var badToken = "badtoken";
            var serviceHttpClient = new JsonHttpClient(_auth.InstanceUrl, _auth.ApiVersion, badToken, new HttpClient());

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

	    [Test]
	    public void CheckInterfaces()
	    {
            using (IAuthenticationClient aa = new AuthenticationClient())
            {
                Assert.IsNotNull(aa);
            }
            using (IJsonHttpClient aa = new JsonHttpClient("instanceUrl", "apiVersion", "accessToken", new HttpClient()))
            {
                Assert.IsNotNull(aa);
            }
            using (IXmlHttpClient aa = new XmlHttpClient("instanceUrl", "apiVersion", "accessToken", new HttpClient()))
            {
                Assert.IsNotNull(aa);
            }
	    }
    }
}
