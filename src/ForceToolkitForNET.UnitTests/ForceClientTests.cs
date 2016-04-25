using System;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Net;
using Salesforce.Force.UnitTests.Models;

namespace Salesforce.Force.UnitTests
{
    [TestFixture]
    public class ForceClientTests
    {
        private const string UserAgent = "forcedotcom-toolkit-dotnet";
        private const string ApiVersion = "v36";

        [Test]
        public async Task Requests_CheckHttpRequestMessage_UserAgent()
        {
            var httpClient = new HttpClient(new ServiceClientRouteHandler(r => Assert.AreEqual(r.Headers.UserAgent.ToString(), UserAgent + string.Format("/{0}", ApiVersion))));
            var forceClient = new ForceClient("http://localhost:1899", "accessToken", ApiVersion, httpClient);

           try
           {
               // suppress error; we only care about checking the header
               await forceClient.QueryAsync<object>("query");
           }
           catch
           {
           }
        }

        [Test]
        public async Task GetBasicInformationAsync_EmptyObjectName_ThrowsException()
        {
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK) {Content = new JsonContent(new {})};
            var httpClient = new HttpClient(new FakeHttpRequestHandler(expectedResponse));
            var forceClient = new ForceClient("http://localhost:1899", "accessToken", ApiVersion, httpClient);

            Action<ArgumentNullException> asserts = exception => Assert.That(exception.Message, Is.Not.Null);
            await AssertEx.ThrowsAsync(() => forceClient.BasicInformationAsync<object>(""), asserts);
        }

        [Test]
        public async Task GetBasicInformationAsync_ValidObjectName_ReturnsParsedResponse()
        {
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.FromFile("KnownGoodContent/UserObjectDescribeMetadata.json")
            };
            var httpClient = new HttpClient(new FakeHttpRequestHandler(expectedResponse));
            var forceClient = new ForceClient("http://localhost:1899", "accessToken", ApiVersion, httpClient);

            var result = await forceClient.BasicInformationAsync<ObjectDescribeMetadata>("ValidObjectName");

            Assert.That(result.Name, Is.Not.Null.And.Not.Empty);
            Assert.AreEqual("User", result.Name);
        }
    }
}
