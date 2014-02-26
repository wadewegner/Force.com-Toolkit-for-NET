using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Web;
using NUnit.Framework;
using Salesforce.Common.Models;

namespace Salesforce.Common.FunctionalTests
{
    public class CommonTests
    {
        private static string _tokenRequestEndpointUrl = ConfigurationSettings.AppSettings["TokenRequestEndpointUrl"];
        private static string _securityToken = ConfigurationSettings.AppSettings["SecurityToken"];
        private static string _consumerKey = ConfigurationSettings.AppSettings["ConsumerKey"];
        private static string _consumerSecret = ConfigurationSettings.AppSettings["ConsumerSecret"];
        private static string _username = ConfigurationSettings.AppSettings["Username"];
        private static string _password = ConfigurationSettings.AppSettings["Password"] + _securityToken;


        [Test]
        public async void Query_Describe()
        {
            const string userAgent = "common-libraries-dotnet";

            var auth = new AuthenticationClient();
            await auth.UsernamePasswordAsync(_consumerKey, _consumerSecret, _username, _password, userAgent, _tokenRequestEndpointUrl);

            var serviceHttpClient = new ServiceHttpClient(auth.InstanceUrl, auth.ApiVersion, auth.AccessToken, userAgent, new HttpClient());
            var objectName = "Account";
            var response = await serviceHttpClient.HttpGetAsync<dynamic>(string.Format("sobjects/{0}", objectName));
            
            Assert.IsNotNull(response);

        }

        [Test]
        public async void Query_Objects()
        {
            const string userAgent = "common-libraries-dotnet";

            var auth = new AuthenticationClient();
            await auth.UsernamePasswordAsync(_consumerKey, _consumerSecret, _username, _password, userAgent, _tokenRequestEndpointUrl);

            var serviceHttpClient = new ServiceHttpClient(auth.InstanceUrl, auth.ApiVersion, auth.AccessToken, userAgent, new HttpClient());

            var response = await serviceHttpClient.HttpGetAsync<DescribeGlobalResult<dynamic>>(string.Format("sobjects"));

            Assert.IsTrue(response.maxBatchSize > 0);
            Assert.IsTrue(response.sobjects.Count > 0);
        }

        [Test]
        public async void Query_Select_Account()
        {
            const string userAgent = "common-libraries-dotnet";

            var auth = new AuthenticationClient();
            await auth.UsernamePasswordAsync(_consumerKey, _consumerSecret, _username, _password, userAgent, _tokenRequestEndpointUrl);

            var serviceHttpClient = new ServiceHttpClient(auth.InstanceUrl, auth.ApiVersion, auth.AccessToken, userAgent, new HttpClient());

            var query = "SELECT id FROM Account";
            var response = await serviceHttpClient.HttpGetAsync<QueryResult<dynamic>>(string.Format("query?q={0}", query));

            Assert.IsTrue(response.totalSize > 0);
            Assert.IsTrue(response.records.Count > 0);
        }

        [Test]
        public async void Query_Select_Count()
        {
            const string userAgent = "common-libraries-dotnet";

            var auth = new AuthenticationClient();
            await auth.UsernamePasswordAsync(_consumerKey, _consumerSecret, _username, _password, userAgent, _tokenRequestEndpointUrl);

            var serviceHttpClient = new ServiceHttpClient(auth.InstanceUrl, auth.ApiVersion, auth.AccessToken, userAgent, new HttpClient());

            var query = "SELECT count() FROM Account";
            var response = await serviceHttpClient.HttpGetAsync<QueryResult<dynamic>>(string.Format("query?q={0}", query));

            Assert.IsTrue(response.totalSize > 0);
            Assert.IsTrue(response.records.Count == 0);
        }

        [Test]
        public async void Auth_UsernamePassword_HasAccessToken()
        {
            const string userAgent = "common-libraries-dotnet";

            var auth = new AuthenticationClient();
            await auth.UsernamePasswordAsync(_consumerKey, _consumerSecret, _username, _password, userAgent, _tokenRequestEndpointUrl);

            Assert.IsNotNullOrEmpty(auth.AccessToken);
        }

        [Test]
        public async void Auth_UsernamePassword_HasInstanceUrl()
        {
            const string userAgent = "common-libraries-dotnet";

            var auth = new AuthenticationClient();
            await auth.UsernamePasswordAsync(_consumerKey, _consumerSecret, _username, _password, userAgent, _tokenRequestEndpointUrl);

            Assert.IsNotNullOrEmpty(auth.InstanceUrl);
        }

        [Test]
        public async void Auth_InvalidLogin()
        {
            const string userAgent = "common-libraries-dotnet";

            try
            {
                var auth = new AuthenticationClient();
                await auth.UsernamePasswordAsync(_consumerKey, _consumerSecret, _username, "WRONGPASSWORD", userAgent, _tokenRequestEndpointUrl);
            }
            catch (ForceAuthException ex)
            {
                Assert.IsNotNull(ex);
                Assert.IsNotNull(ex.Message);
                Assert.IsNotNull(ex.Error);
            }
        }

    }
}
