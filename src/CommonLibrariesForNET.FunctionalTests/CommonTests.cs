using System.Configuration;
using System.Net.Http;
using NUnit.Framework;
using Salesforce.Common.Models;

namespace Salesforce.Common.FunctionalTests
{
	[TestFixture]
    public class CommonTests
    {
#pragma warning disable 618
        private static string _tokenRequestEndpointUrl = ConfigurationSettings.AppSettings["TokenRequestEndpointUrl"];
        private static string _securityToken = ConfigurationSettings.AppSettings["SecurityToken"];
        private static string _consumerKey = ConfigurationSettings.AppSettings["ConsumerKey"];
        private static string _consumerSecret = ConfigurationSettings.AppSettings["ConsumerSecret"];
        private static string _username = ConfigurationSettings.AppSettings["Username"];
        private static string _password = ConfigurationSettings.AppSettings["Password"] + _securityToken;
#pragma warning enable 618

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

        [Test]
        // Test assumes the RestResource Apex class in this doc has been created. http://www.salesforce.com/us/developer/docs/apexcode/Content/apex_rest_code_sample_basic.htm
        public async void ApexRest_ServiceType_Create_Query_Delete()
        {
            const string userAgent = "common-libraries-dotnet";

            var auth = new AuthenticationClient();
            await auth.UsernamePasswordAsync(_consumerKey, _consumerSecret, _username, _password, userAgent, _tokenRequestEndpointUrl);

            var serviceHttpClient = new ServiceHttpClient(auth.InstanceUrl, auth.ApiVersion, auth.AccessToken, userAgent, ServiceType.ApexRest, new HttpClient());

            var testAccount = new
            {
                name = "test name",
            };

            // Create test account.
            var accountId = await serviceHttpClient.HttpPostAsync<string>(testAccount,"Account");

            Assert.IsNotNullOrEmpty(accountId);

            // Query test account
            var account = await serviceHttpClient.HttpGetAsync<dynamic>(string.Format("Account/{0}", accountId));

            Assert.IsTrue(account.Name == testAccount.name);

            // Delete test account
            var deleteResponse = await serviceHttpClient.HttpDeleteAsync(string.Format("Account/{0}", accountId));

            Assert.IsTrue(deleteResponse);
        }

        [Test]
        public async void Data_ServiceType_Create_Query_Delete()
        {
            const string userAgent = "common-libraries-dotnet";

            var auth = new AuthenticationClient();
            await auth.UsernamePasswordAsync(_consumerKey, _consumerSecret, _username, _password, userAgent, _tokenRequestEndpointUrl);

            var serviceHttpClient = new ServiceHttpClient(auth.InstanceUrl, auth.ApiVersion, auth.AccessToken, userAgent, ServiceType.Data, new HttpClient());

            var account = new { Name = "New Account", Description = "New Account Description" };

            // Create account.
            var createResponse = await serviceHttpClient.HttpPostAsync<SuccessResponse>(account, "sobjects/Account");
            Assert.IsNotNull(createResponse);

            // Query account.
            var result = await serviceHttpClient.HttpGetAsync<QueryResult<dynamic>>(string.Format("query?q=SELECT ID FROM ACCOUNT WHERE Id = '{0}'", createResponse.id));
            Assert.IsNotNull(result);

            // Delete account.
            var deleteResponse = await serviceHttpClient.HttpDeleteAsync(string.Format("sobjects/{0}/{1}", "Account", createResponse.id));
            Assert.IsTrue(deleteResponse);
        }

        [Test]
        public void FormatUrl_With_ServiceType_Data()
        {
            const string resourceName = "Account";
            const string instanceUrl = "http://localhost/";
            const string apiVersion = "1.0";

            var url = Common.FormatUrl(resourceName, instanceUrl, apiVersion, ServiceType.Data);

            var expectedResult = string.Format("{0}/services/{1}/{2}/{3}", instanceUrl, ServiceType.Data.ToString().ToLower(), apiVersion, resourceName);

            Assert.AreEqual(url, expectedResult);
        }

        [Test]
        public void FormatUrl_With_ServiceType_ApexRest()
        {
            const string resourceName = "Account";
            const string instanceUrl = "http://localhost/";
            const string apiVersion = "1.0";

            var url = Common.FormatUrl(resourceName, instanceUrl, apiVersion, ServiceType.ApexRest);

            var expectedResult = string.Format("{0}/services/{1}/{2}", instanceUrl, ServiceType.ApexRest.ToString().ToLower(), resourceName);

            Assert.AreEqual(url, expectedResult);
        }

        [Test]
        public void FormatUrl_With_Default_ServiceType()
        {
            const string resourceName = "Account";
            const string instanceUrl = "http://localhost/";
            const string apiVersion = "1.0";

            var url = Common.FormatUrl(resourceName, instanceUrl, apiVersion);

            var expectedResult = string.Format("{0}/services/{1}/{2}/{3}", instanceUrl, ServiceType.Data.ToString().ToLower(), apiVersion, resourceName);

            Assert.AreEqual(url, expectedResult);
        }
    }
}
