using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
            Assert.That(auth.ApiVersion, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task Auth_UsernamePassword_Check()
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

            using (IAuthenticationClient auth = new AuthenticationClient(client))
            {
                await auth.UsernamePasswordAsync(consumerKey, consumerSecret, username, password);

                Assert.IsNotNull(auth.AccessToken);
                Assert.IsNotNull(auth.ApiVersion);
                Assert.IsNotNull(auth.InstanceUrl);
                Assert.AreEqual(auth.AccessToken, "AccessToken");
                Assert.AreEqual(auth.ApiVersion, "v36.0");
                Assert.AreEqual(auth.InstanceUrl, "InstanceUrl");
            }
        }

        [Test]
        public async Task Requests_CheckHttpRequestMessage_HttpGet()
        {
            var client = new HttpClient(new ServiceClientRouteHandler(r =>
            {
                Assert.AreEqual(r.RequestUri.ToString(), "http://localhost:1899/services/data/v36/wade");

                Assert.IsNotNull(r.Headers.UserAgent);
                Assert.AreEqual(r.Headers.UserAgent.ToString(), UserAgent + "/v36");

                Assert.IsNotNull(r.Headers.Authorization);
                Assert.AreEqual(r.Headers.Authorization.ToString(), "Bearer accessToken");
            }));

            using (IJsonHttpClient httpClient = new JsonHttpClient("http://localhost:1899", "v36", "accessToken", client))
            {
                await httpClient.HttpGetAsync<object>("wade");
            }
        }

        [Test]
        public async Task Requests_CheckHttpRequestMessage_HttpGet_WithNode()
        {
            var client = new HttpClient(new ServiceClientRouteHandler(r =>
            {
                Assert.AreEqual(r.RequestUri.ToString(), "http://localhost:1899/services/data/v36/wade");

                Assert.IsNotNull(r.Headers.UserAgent);
                Assert.AreEqual(r.Headers.UserAgent.ToString(), UserAgent + "/v36");

                Assert.IsNotNull(r.Headers.Authorization);
                Assert.AreEqual(r.Headers.Authorization.ToString(), "Bearer accessToken");
            }));

            using (IJsonHttpClient httpClient = new JsonHttpClient("http://localhost:1899", "v36", "accessToken", client))
            {
                await httpClient.HttpGetAsync<object>("wade");
            }
        }

        [Test]
        public async Task Requests_CheckHttpRequestMessage_HttpPost()
        {
            var client = new HttpClient(new ServiceClientRouteHandler(r =>
            {
                Assert.AreEqual(r.RequestUri.ToString(), "http://localhost:1899/services/data/v36/wade");

                Assert.IsNotNull(r.Headers.UserAgent);
                Assert.AreEqual(r.Headers.UserAgent.ToString(), UserAgent + "/v36");

                Assert.IsNotNull(r.Headers.Authorization);
                Assert.AreEqual(r.Headers.Authorization.ToString(), "Bearer accessToken");
            }));

            using (IJsonHttpClient httpClient = new JsonHttpClient("http://localhost:1899", "v36", "accessToken", client))
            {
                await httpClient.HttpPostAsync<object>(null, "wade");
            }
        }

        [Test]
        public async Task Requests_CheckHttpRequestMessage_HttpPatch()
        {
            var client = new HttpClient(new ServiceClientRouteHandler(r =>
            {
                Assert.AreEqual(r.RequestUri.ToString(), "http://localhost:1899/services/data/v36/wade");

                Assert.IsNotNull(r.Headers.UserAgent);
                Assert.AreEqual(r.Headers.UserAgent.ToString(), UserAgent + "/v36");

                Assert.IsNotNull(r.Headers.Authorization);
                Assert.AreEqual(r.Headers.Authorization.ToString(), "Bearer accessToken");
            }));

            using (IJsonHttpClient httpClient = new JsonHttpClient("http://localhost:1899", "v36", "accessToken", client))
            {
                await httpClient.HttpPatchAsync(null, "wade");
            }
        }

        [Test]
        public async Task Requests_CheckHttpRequestMessage_HttpDelete()
        {
            var client = new HttpClient(new ServiceClientRouteHandler(r =>
            {
                Assert.AreEqual(r.RequestUri.ToString(), "http://localhost:1899/services/data/v36/wade");

                Assert.IsNotNull(r.Headers.UserAgent);
                Assert.AreEqual(r.Headers.UserAgent.ToString(), UserAgent + "/v36");

                Assert.IsNotNull(r.Headers.Authorization);
                Assert.AreEqual(r.Headers.Authorization.ToString(), "Bearer accessToken");
            }));

            using (IJsonHttpClient httpClient = new JsonHttpClient("http://localhost:1899", "v36", "accessToken", client))
            {
                await httpClient.HttpDeleteAsync("wade");
            }
        }

        [Test]
        public async Task Requests_CheckCustomRequestsHeaders()
        {
            var client = new HttpClient(new ServiceClientRouteHandler(r =>
            {
                Assert.IsNotNull(r.Headers.GetValues("headername"));
                Assert.AreEqual(r.Headers.GetValues("headername").FirstOrDefault(), "headervalue");
            }));

            client.DefaultRequestHeaders.Add("headername", "headervalue");

            using (IJsonHttpClient httpClient = new JsonHttpClient("http://localhost:1899", "v36", "accessToken", client))
            {
                await httpClient.HttpGetAsync<object>("wade");
            }
        }

        [Test]
        public void NewServiceHttpClient_ResetsUserAgents()
        {
            var httpClientUserAgent = UserAgent + "/v36";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(httpClientUserAgent);
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(httpClientUserAgent);
            Assert.AreEqual(httpClientUserAgent + " " + httpClientUserAgent, httpClient.DefaultRequestHeaders.UserAgent.ToString());

            var serviceClient = new JsonHttpClient("http://localhost:1899", "v36", "accessToken", httpClient);

            // Ensure the old user agent header is replaced.
            Assert.AreEqual(httpClientUserAgent, httpClient.DefaultRequestHeaders.UserAgent.ToString());
        }
    }
}
