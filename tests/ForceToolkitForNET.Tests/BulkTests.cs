using System.IO;
using System.Linq;
using System.Net.Http;
using System.Xml;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NUnit.Framework;
using Salesforce.Common;

namespace Salesforce.Force.Tests
{
    [TestFixture]
    public class BulkTests
    {
        private const string UserAgent = "forcedotcom-toolkit-dotnet";

        [Test]
        public async Task Requests_CheckHttpRequestMessage_HttpGetXml()
        {
            var client = new HttpClient(new BulkServiceClientRouteHandler(r =>
            {
                // the v should be removed...
                Assert.AreEqual(r.RequestUri.ToString(), "http://localhost:1899/services/data/32/brad");

                Assert.IsNotNull(r.Headers.UserAgent);
                Assert.AreEqual(r.Headers.UserAgent.ToString(), UserAgent + "/v32");

                Assert.IsNotNull(r.Headers.GetValues("X-SFDC-Session"));
                Assert.IsTrue(r.Headers.GetValues("X-SFDC-Session").Count() == 1);
                Assert.AreEqual(r.Headers.GetValues("X-SFDC-Session").First(), "accessToken");
            }, new object()));

            using (var httpClient = new XmlHttpClient("http://localhost:1899", "v32", "accessToken", client))
            {
                await httpClient.HttpGetAsync<object>("brad");
            }
        }

        [Test]
        public async Task Requests_CheckHttpRequestMessageAndResponseDeserialization_HttpPostXml()
        {
            var testObject = new SerializerTest { TestField = "testMessage" };
            var client = new HttpClient(new BulkServiceClientRouteHandler(r =>
            {
                // the v should be removed...
                Assert.AreEqual(r.RequestUri.ToString(), "http://localhost:1899/services/data/32/brad");

                Assert.IsNotNull(r.Headers.UserAgent);
                Assert.AreEqual(r.Headers.UserAgent.ToString(), UserAgent + "/v32");

                Assert.IsNotNull(r.Headers.GetValues("X-SFDC-Session"));
                Assert.IsTrue(r.Headers.GetValues("X-SFDC-Session").Count() == 1);
                Assert.AreEqual(r.Headers.GetValues("X-SFDC-Session").First(), "accessToken");

                // check the object is serialized as expected
                var serializedPayload = MimicSerialization(testObject);
                var stringContent = r.Content.ReadAsStringAsync().Result;
                Assert.AreEqual(serializedPayload, stringContent);

            }, testObject)); // pass in the object to be returned (Same object roundtripped here for simplicity)

            using (var httpClient = new XmlHttpClient("http://localhost:1899", "v32", "accessToken", client))
            {
                var result = await httpClient.HttpPostAsync<SerializerTest>(testObject, "brad");
                Assert.AreEqual(testObject.TestField, result.TestField);
            }
        }

        [XmlRoot(ElementName = "test_Object")]
        public class SerializerTest
        {
            [XmlElement(ElementName = "test_Field")]
            public string TestField { get; set; }
        }

        private static string MimicSerialization(object inputObject)
        {
            var xmlSerializer = new XmlSerializer(inputObject.GetType());
            var stringWriter = new StringWriter();
            string result;
            using (var writer = XmlWriter.Create(stringWriter))
            {
                xmlSerializer.Serialize(writer, inputObject);
                result = stringWriter.ToString();
            }
            return result;
        }
    }
}
