using System;
using System.Net.Http;
using NUnit.Framework;
using System.Net;
using Salesforce.Force.UnitTests.Models;

namespace Salesforce.Force.UnitTests
{
    [TestFixture]
    public class ForceClientTests
    {
        private const string UserAgent = "forcedotcom-toolkit-dotnet";

        [Test]
        public async void Requests_CheckHttpRequestMessage_UserAgent()
        {
            var httpClient = new HttpClient(new ServiceClientRouteHandler(r => Assert.AreEqual(r.Headers.UserAgent.ToString(), UserAgent + "/v32")));
            var forceClient = new ForceClient("http://localhost:1899", "accessToken", "v32", httpClient, new HttpClient());

           try
           {
               // suppress error; we only care about checking the header
               await forceClient.QueryAsync<object>("query");
           }
           catch
           {
               // do nothing
           }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void GetBasicInformationAsync_EmptyObjectName_ThrowsException()
        {
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK) {Content = new JsonContent(new {})};
            var httpClient = new HttpClient(new FakeHttpRequestHandler(expectedResponse));
            var forceClient = new ForceClient("http://localhost:1899", "accessToken", "v29", httpClient, null);

            await forceClient.BasicInformationAsync<object>("");

            // expects exception
        }

        [Test]
        public async void GetBasicInformationAsync_ValidObjectName_ReturnsParsedResponse()
        {
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.FromFile("KnownGoodContent/UserObjectDescribeMetadata.json")
            };
            var httpClient = new HttpClient(new FakeHttpRequestHandler(expectedResponse));
            var forceClient = new ForceClient("http://localhost:1899", "accessToken", "v29", httpClient, new HttpClient());

            var result = await forceClient.BasicInformationAsync<ObjectDescribeMetadata>("ValidObjectName");

            Assert.IsNotNullOrEmpty(result.Name);
            Assert.AreEqual("User", result.Name);
        }
    }
}
