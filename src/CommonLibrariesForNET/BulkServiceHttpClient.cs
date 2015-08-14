using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Salesforce.Common.Models.Xml;

namespace Salesforce.Common
{
    public class BulkServiceHttpClient : IBulkServiceHttpClient, IDisposable
    {
        private const string UserAgent = "forcedotcom-toolkit-dotnet";
        private readonly string _instanceUrl;
        private readonly string _apiVersion;
        private readonly HttpClient _httpClient;

        public BulkServiceHttpClient(string instanceUrl, string apiVersion, string accessToken, HttpClient httpClient)
        {
            if (string.IsNullOrEmpty(instanceUrl)) throw new ArgumentNullException("instanceUrl");
            if (string.IsNullOrEmpty(apiVersion)) throw new ArgumentNullException("apiVersion");
            if (string.IsNullOrEmpty(accessToken)) throw new ArgumentNullException("accessToken");
            if (httpClient == null) throw new ArgumentNullException("httpClient");

            _instanceUrl = instanceUrl;
            _apiVersion = apiVersion;
            _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(string.Concat(UserAgent, "/", _apiVersion));

            if (_apiVersion.StartsWith("v", StringComparison.OrdinalIgnoreCase))
            {
                _apiVersion = _apiVersion.Substring(1);
            }

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            _httpClient.DefaultRequestHeaders.Add("X-SFDC-Session", accessToken);

        }

        public async Task<T> HttpPostXmlAsync<T>(object inputObject, string urlSuffix)
        {
            var url = Common.FormatUrl(urlSuffix, _instanceUrl, _apiVersion);
            var postBody = SerializeXmlObject(inputObject);
            var content = new StringContent(postBody, Encoding.UTF8, "application/xml");

            var responseMessage = await _httpClient.PostAsync(new Uri(url), content).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode)
            {
                return DeserializeXmlString<T>(response);
            }

            var errorResponse = DeserializeXmlString<ErrorResponse>(response);
            throw new ForceException(errorResponse.ErrorCode, errorResponse.Message);
        }

        public async Task<T> HttpGetXmlAsync<T>(string urlSuffix)
        {
            var url = Common.FormatUrl(urlSuffix, _instanceUrl, _apiVersion);
            var responseMessage = await _httpClient.GetAsync(new Uri(url)).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode)
            {
                return DeserializeXmlString<T>(response);
            }

            var errorResponse = DeserializeXmlString<ErrorResponse>(response);
            throw new ForceException(errorResponse.ErrorCode, errorResponse.Message);
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

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
