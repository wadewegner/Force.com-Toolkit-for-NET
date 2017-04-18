using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Salesforce.Common.Internals;
using Salesforce.Common.Models.Json;

namespace Salesforce.Common
{
    public class XmlHttpClient : BaseHttpClient, IXmlHttpClient
    {

        public XmlHttpClient(string instanceUrl, string apiVersion, string accessToken, HttpClient httpClient)
            : base(instanceUrl, apiVersion, "application/xml", httpClient)
        {
            if (ApiVersion.StartsWith("v", StringComparison.OrdinalIgnoreCase))
            {
                ApiVersion = ApiVersion.Substring(1);
            }
            HttpClient.DefaultRequestHeaders.Add("X-SFDC-Session", accessToken);
        }

        // GET

        public Task<T> HttpGetAsync<T>(string urlSuffix, CancellationToken token)
        {
            var url = Common.FormatUrl(urlSuffix, InstanceUrl, ApiVersion);
            return HttpGetAsync<T>(url, token);
        }

        public async Task<T> HttpGetAsync<T>(Uri uri, CancellationToken token)
        {
            try
            {
                var response = await HttpGetAsync(uri, token).ConfigureAwait(false);
                return DeserializeXmlString<T>(response);
            }
            catch (BaseHttpClientException e)
            {
                throw ParseForceException(e.Message);
            }
        }

        // POST

        public Task<T> HttpPostAsync<T>(object inputObject, string urlSuffix, CancellationToken token)
        {
            var url = Common.FormatUrl(urlSuffix, InstanceUrl, ApiVersion);
            return HttpPostAsync<T>(inputObject, url, token);
        }

        public async Task<T> HttpPostAsync<T>(object inputObject, Uri uri, CancellationToken token)
        {
            var postBody = SerializeXmlObject(inputObject);
            try
            {
                var response = await HttpPostAsync(postBody, uri, token).ConfigureAwait(false);
                return DeserializeXmlString<T>(response);
            }
            catch (BaseHttpClientException e)
            {
                throw ParseForceException(e.Message);
            }
        }

        // HELPER METHODS

        private static ForceException ParseForceException(string responseMessage)
        {
            var errorResponse = DeserializeXmlString<ErrorResponse>(responseMessage);
            return new ForceException(errorResponse.ErrorCode, errorResponse.Message);
        }

        private static string SerializeXmlObject(object inputObject)
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

        private static T DeserializeXmlString<T>(string inputString)
        {
            var serializer = new XmlSerializer(typeof(T));
            T result;
            using (TextReader reader = new StringReader(inputString))
            {
                result = (T) serializer.Deserialize(reader);
            }
            return result;
        }
    }
}
