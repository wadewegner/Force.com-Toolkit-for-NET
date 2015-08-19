using System.Linq;
using System.Net.Http;
using NUnit.Framework;

namespace Salesforce.Common.UnitTests
{
    [TestFixture]
    public class BulkTests
    {
        private const string UserAgent = "forcedotcom-toolkit-dotnet";

        [Test]
        public async void Requests_CheckHttpRequestMessage_HttpGetXml()
        {
            var client = new HttpClient(new BulkServiceClientRouteHandler(r =>
            {
                Assert.AreEqual(r.RequestUri.ToString(), "http://localhost:1899/services/data/32/brad");

                Assert.IsNotNull(r.Headers.UserAgent);
                Assert.AreEqual(r.Headers.UserAgent.ToString(), UserAgent + "/v32");

                Assert.IsNotNull(r.Headers.GetValues("X-SFDC-Session"));
                Assert.IsTrue(r.Headers.GetValues("X-SFDC-Session").Count() == 1);
                Assert.AreEqual(r.Headers.GetValues("X-SFDC-Session").First(), "accessToken");
            }, new object()));

            using (var httpClient = new BulkServiceHttpClient("http://localhost:1899", "v32", "accessToken", client))
            {
                await httpClient.HttpGetXmlAsync<object>("brad");
            }
        }
    }
}
