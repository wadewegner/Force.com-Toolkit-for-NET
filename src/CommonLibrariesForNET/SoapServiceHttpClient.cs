using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Salesforce.Common.Models;

namespace Salesforce.Common
{
    public class SoapServiceHttpClient : ISoapServiceHttpClient, IDisposable
    {
        private const string UserAgent = "forcedotcom-toolkit-dotnet";
        private readonly string _instanceUrl;
        private readonly string _apiVersion;
        private readonly HttpClient _httpClient;

        public SoapServiceHttpClient(string instanceUrl, string apiVersion, string accessToken, HttpClient httpClient)
        {
            _instanceUrl = instanceUrl;
            _apiVersion = apiVersion;
            _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(string.Concat(UserAgent, "/", _apiVersion));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        }

        public async Task<T> HttpPostAsync<T>(object inputObject, string urlSuffix)
        {
            SetXmlHeader();

            var url = Common.FormatUrl(urlSuffix, _instanceUrl, _apiVersion);
            var postBody = SerializeXmlObject(inputObject);
            var content = new StringContent(postBody, Encoding.UTF8, "application/xml");

            var responseMessage = await _httpClient.PostAsync(new Uri(url), content).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode)
            {
                return DeserializeXmlString<T>(response);
            }

            var errorResponse = JsonConvert.DeserializeObject<ErrorResponses>(response);
            throw new ForceException(errorResponse[0].ErrorCode, errorResponse[0].Message);
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

        private void SetXmlHeader()
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
