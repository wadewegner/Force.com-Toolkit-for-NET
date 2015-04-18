using System.Linq;
using System.Net.Http;
using NUnit.Framework;

namespace Salesforce.Common.UnitTests
{
    [TestFixture]
    public class CommonTests
    {
        private const string UserAgent = "forcedotcom-toolkit-dotnet";

        [Test]
        public void Auth_HasApiVersion()
        {
            var auth = new AuthenticationClient();
            Assert.IsNotNullOrEmpty(auth.ApiVersion);
        }

        [Test]
        public async void Auth_UsernamePassword_Check()
        {
            const string consumerKey = "CONSUMERKEY";
            const string consumerSecret = "CONSUMERSECRET";
            const string username = "USERNAME";
            const string password = "PASSWORD";

            var client = new HttpClient(new AuthenticationClientRouteHandler(r =>
            {
                Assert.IsNotNull(r.Content);
                Assert.AreEqual(r.Content.ToString(), "System.Net.Http.FormUrlEncodedContent");
            }));

            using (var auth = new AuthenticationClient(client))
            {
                await auth.UsernamePasswordAsync(consumerKey, consumerSecret, username, password);

                Assert.IsNotNull(auth.AccessToken);
                Assert.IsNotNull(auth.ApiVersion);
                Assert.IsNotNull(auth.InstanceUrl);
                Assert.AreEqual(auth.AccessToken, "AccessToken");
                Assert.AreEqual(auth.ApiVersion, "v32.0");
                Assert.AreEqual(auth.InstanceUrl, "InstanceUrl");
            }
        }

        [Test]
        public async void Requests_CheckHttpRequestMessage_HttpGet()
        {
            var client = new HttpClient(new ServiceClientRouteHandler(r =>
            {
                Assert.AreEqual(r.RequestUri.ToString(), "http://localhost:1899/services/data/v32/wade");

                Assert.IsNotNull(r.Headers.UserAgent);
                Assert.AreEqual(r.Headers.UserAgent.ToString(), UserAgent + "/v32");

                Assert.IsNotNull(r.Headers.Authorization);
                Assert.AreEqual(r.Headers.Authorization.ToString(), "Bearer accessToken");
            }));

            using (var httpClient = new ServiceHttpClient("http://localhost:1899", "v32", "accessToken", client))
            {
                await httpClient.HttpGetAsync<object>("wade");
            }
        }

        [Test]
        public async void Requests_CheckHttpRequestMessage_HttpGet_WithNode()
        {
            var client = new HttpClient(new ServiceClientRouteHandler(r =>
            {
                Assert.AreEqual(r.RequestUri.ToString(), "http://localhost:1899/services/data/v32/wade");

                Assert.IsNotNull(r.Headers.UserAgent);
                Assert.AreEqual(r.Headers.UserAgent.ToString(), UserAgent + "/v32");

                Assert.IsNotNull(r.Headers.Authorization);
                Assert.AreEqual(r.Headers.Authorization.ToString(), "Bearer accessToken");
            }));

            using (var httpClient = new ServiceHttpClient("http://localhost:1899", "v32", "accessToken", client))
            {
                await httpClient.HttpGetAsync<object>("wade");
            }
        }

        [Test]
        public async void Requests_CheckHttpRequestMessage_HttpPost()
        {
            var client = new HttpClient(new ServiceClientRouteHandler(r =>
            {
                Assert.AreEqual(r.RequestUri.ToString(), "http://localhost:1899/services/data/v32/wade");

                Assert.IsNotNull(r.Headers.UserAgent);
                Assert.AreEqual(r.Headers.UserAgent.ToString(), UserAgent + "/v32");

                Assert.IsNotNull(r.Headers.Authorization);
                Assert.AreEqual(r.Headers.Authorization.ToString(), "Bearer accessToken");
            }));

            using (var httpClient = new ServiceHttpClient("http://localhost:1899", "v32", "accessToken", client))
            {
                await httpClient.HttpPostAsync<object>(null, "wade");
            }
        }

        [Test]
        public async void Requests_CheckHttpRequestMessage_HttpPatch()
        {
            var client = new HttpClient(new ServiceClientRouteHandler(r =>
            {
                Assert.AreEqual(r.RequestUri.ToString(), "http://localhost:1899/services/data/v32/wade");

                Assert.IsNotNull(r.Headers.UserAgent);
                Assert.AreEqual(r.Headers.UserAgent.ToString(), UserAgent + "/v32");

                Assert.IsNotNull(r.Headers.Authorization);
                Assert.AreEqual(r.Headers.Authorization.ToString(), "Bearer accessToken");
            }));

            using (var httpClient = new ServiceHttpClient("http://localhost:1899", "v32", "accessToken", client))
            {
                await httpClient.HttpPatchAsync(null, "wade");
            }
        }

        [Test]
        public async void Requests_CheckHttpRequestMessage_HttpDelete()
        {
            var client = new HttpClient(new ServiceClientRouteHandler(r =>
            {
                Assert.AreEqual(r.RequestUri.ToString(), "http://localhost:1899/services/data/v32/wade");

                Assert.IsNotNull(r.Headers.UserAgent);
                Assert.AreEqual(r.Headers.UserAgent.ToString(), UserAgent + "/v32");

                Assert.IsNotNull(r.Headers.Authorization);
                Assert.AreEqual(r.Headers.Authorization.ToString(), "Bearer accessToken");
            }));

            using (var httpClient = new ServiceHttpClient("http://localhost:1899", "v32", "accessToken", client))
            {
                await httpClient.HttpDeleteAsync("wade");
            }
        }

        [Test]
        public async void Requests_CheckCustomRequestsHeaders()
        {
            var client = new HttpClient(new ServiceClientRouteHandler(r =>
            {
                Assert.IsNotNull(r.Headers.GetValues("headername"));
                Assert.AreEqual(r.Headers.GetValues("headername").FirstOrDefault(), "headervalue");
            }));

            client.DefaultRequestHeaders.Add("headername", "headervalue");

            using (var httpClient = new ServiceHttpClient("http://localhost:1899", "v32", "accessToken", client))
            {
                await httpClient.HttpGetAsync<object>("wade");
            }
        }
    }
}
