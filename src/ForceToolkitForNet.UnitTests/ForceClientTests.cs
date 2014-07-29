//TODO: add license header

using System;
using System.Net.Http;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Net;
using Salesforce.Force.UnitTests.DTO;

namespace Salesforce.Force.UnitTests
{
    [TestFixture]
    public class ForceClientTests
    {
        [Test]
        public async void Requests_CheckHttpRequestMessage_UserAgent()
        {
            var httpClient = new HttpClient(new ServiceClientRouteHandler(r => Assert.AreEqual(r.Headers.UserAgent.ToString(), "forcedotcom-toolkit-dotnet/v30")));
            var forceClient = new ForceClient("http://localhost:1899", "accessToken", "v30", httpClient);

           try
           {
               // suppress error; we only care about checking the header
               var query = await forceClient.QueryAsync<object>("query");
           }
           catch (Exception)
           {
           }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void GetBasicInformationAsync_EmptyObjectName_ThrowsException()
        {
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);
            expectedResponse.Content = new JsonContent(new { });
            var httpClient = new HttpClient(new FakeHttpRequestHandler(expectedResponse));
            var forceClient = new ForceClient("http://localhost:1899", "accessToken", "v29", httpClient);

            var result = await forceClient.GetBasicInformationAsync<object>("");

            // expects exception
        }

        [Test]
        public async void GetBasicInformationAsync_ValidObjectName_ReturnsParsedResponse()
        {
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);
            expectedResponse.Content = JsonContent.FromFile("KnownGoodContent/UserObjectDescribeMetadata.json");
            var httpClient = new HttpClient(new FakeHttpRequestHandler(expectedResponse));
            var forceClient = new ForceClient("http://localhost:1899", "accessToken", "v29", httpClient);

            var result = await forceClient.GetBasicInformationAsync<ObjectDescribeMetadata>("ValidObjectName");

            Assert.IsNotNullOrEmpty(result.Name);
            Assert.AreEqual("User", result.Name);
        }

    }
}
