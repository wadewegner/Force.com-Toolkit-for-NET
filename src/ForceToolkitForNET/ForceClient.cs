using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text;
using CommonToolkitForNET;
using CommonToolkitForNET.Models;
using ForceToolkitForNET.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ForceToolkitForNET
{
    public class ForceClient : IForceClient
    {
        public string InstanceUrl { get; set; }
        public string AccessToken { get; set; }
        public string ApiVersion { get; set; }

        private static ToolkitHttpClientTest _toolkitHttpClientTest;

        private static ToolkitHttpClient _toolkitHttpClient;
        
        public ForceClient(string instanceUrl, string accessToken, string apiVersion)
        {
            this.InstanceUrl = instanceUrl;
            this.AccessToken = accessToken;
            this.ApiVersion = apiVersion;

            const string userAgent = "salesforce-toolkit-dotnet";

            _toolkitHttpClient = new ToolkitHttpClient(instanceUrl, apiVersion, accessToken, userAgent);
            _toolkitHttpClientTest = new ToolkitHttpClientTest(instanceUrl, apiVersion, accessToken, userAgent);
        }
      
        public async Task<IList<T>> Query<T>(string query)
        {
            var response = await _toolkitHttpClient.HttpGet<IList<T>>(string.Format("query?q={0}", query), "records");
            return response;
        }

        public async Task<string> Create(string objectName, object record)
        {
            var response = await _toolkitHttpClient.HttpPost<SuccessResponse>(record, string.Format("sobjects/{0}", objectName));
            return response.id;
        }

        public async Task<bool> Update(string objectName, string recordId, object record)
        {
            var response = await _toolkitHttpClient.HttpPatch(record, string.Format("sobjects/{0}/{1}", objectName, recordId));
            return response;
        }

        public async Task<bool> Delete(string objectName, string recordId)
        {
            var response = await _toolkitHttpClient.HttpDelete(string.Format("sobjects/{0}/{1}", objectName, recordId));
            return response;
        }

        public async Task<T> QueryById<T>(string objectName, string recordId)
        {
            var fields = string.Join(", ", typeof(T).GetProperties().Select(p => p.Name));
            var query = string.Format("SELECT {0} FROM {1} WHERE Id = '{2}'", fields, objectName, recordId);
            var results = await Query<T>(query);

            return results.FirstOrDefault();
        }

        public async Task<List<T>> GetObjects<T>()
        {
            var url = string.Format("{0}", Common.FormatUrl("sobjects", this.InstanceUrl, this.ApiVersion));

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(string.Format("salesforce-toolkit-dotnet/{0}",
                    ApiVersion));

                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get
                };

                request.Headers.Add("Authorization", "Bearer " + AccessToken);

                var responseMessage = await client.SendAsync(request);
                var response = await responseMessage.Content.ReadAsStringAsync();

                if (responseMessage.IsSuccessStatusCode)
                {
                    var jObject = JObject.Parse(response);
                    var jToken = jObject.GetValue("sobjects");

                    var r = JsonConvert.DeserializeObject<List<T>>(jToken.ToString());
                    return r;
                }

                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(response);
                throw new ForceException(errorResponse.error, errorResponse.error_description);
            }
        }

        public async Task<T> Describe<T>(string objectName)
        {
            var url = string.Format("{0}/{1}", Common.FormatUrl("sobjects", this.InstanceUrl, this.ApiVersion), objectName);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(string.Format("salesforce-toolkit-dotnet/{0}",
                    ApiVersion));

                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get
                };

                request.Headers.Add("Authorization", "Bearer " + AccessToken);

                var responseMessage = await client.SendAsync(request);
                var response = await responseMessage.Content.ReadAsStringAsync();

                if (responseMessage.IsSuccessStatusCode)
                {
                    var jObject = JObject.Parse(response);
                    var jToken = jObject.GetValue("objectDescribe");

                    var r = JsonConvert.DeserializeObject<T>(jToken.ToString());
                    return r;
                }

                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(response);
                throw new ForceException(errorResponse.error, errorResponse.error_description);
            }
        }

    }
}
