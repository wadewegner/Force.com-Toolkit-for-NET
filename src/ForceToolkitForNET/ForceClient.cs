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
        
        public ForceClient(string instanceUrl, string accessToken, string apiVersion)
        {
            this.InstanceUrl = instanceUrl;
            this.AccessToken = accessToken;
            this.ApiVersion = apiVersion;
        }
      
        public async Task<IList<T>> Query<T>(string query)
        {
            var url = string.Format("{0}?q={1}", Common.FormatUrl("query", this.InstanceUrl, this.ApiVersion), query);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(string.Format("salesforce-toolkit-dotnet/{0}", ApiVersion));

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
                    var jToken = jObject.GetValue("records");

                    var r = JsonConvert.DeserializeObject<IList<T>>(jToken.ToString());
                    return r;
                }

                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(response);
                throw new ForceException(errorResponse.error, errorResponse.error_description);
            }
        }

        public async Task<string> Create(string objectName, object record)
        {
            var url = Common.FormatUrl("sobjects", this.InstanceUrl, this.ApiVersion) + "/" + objectName;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.UserAgent.ParseAdd(string.Format("salesforce-toolkit-dotnet/{0}",
                    ApiVersion));

                var json = JsonConvert.SerializeObject(record);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var responseMessage = await client.PostAsync(url, content);
                var response = await responseMessage.Content.ReadAsStringAsync();

                if (responseMessage.IsSuccessStatusCode)
                {
                    var id = JsonConvert.DeserializeObject<SuccessResponse>(response).id;
                    return id;
                }

                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(response);
                throw new ForceException(errorResponse.message, errorResponse.errorCode);
            }
        }

        public async Task<bool> Update(string objectName, string recordId, object record)
        {
            var url = Common.FormatUrl("sobjects", this.InstanceUrl, this.ApiVersion) + "/" + objectName + "/" + recordId;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(string.Format("salesforce-toolkit-dotnet/{0}",
                    ApiVersion));

                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(url),
                    Method = new HttpMethod("PATCH")
                };

                var json = JsonConvert.SerializeObject(record);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                request.Content = content;
                request.Headers.Add("Authorization", "Bearer " + AccessToken);

                var responseMessage = await client.SendAsync(request);

                if (responseMessage.IsSuccessStatusCode)
                {
                    return true;
                }

                var response = await responseMessage.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(response);
                throw new ForceException(errorResponse.error, errorResponse.error_description);
            }
        }

        public async Task<bool> Delete(string objectName, string recordId)
        {
            var url = Common.FormatUrl("sobjects", this.InstanceUrl, this.ApiVersion) + "/" + objectName + "/" + recordId;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(string.Format("salesforce-toolkit-dotnet/{0}", ApiVersion));

                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Delete
                };

                request.Headers.Add("Authorization", "Bearer " + AccessToken);

                var responseMessage = await client.SendAsync(request);

                if (responseMessage.IsSuccessStatusCode)
                {
                    return true;
                }

                var response = await responseMessage.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(response);
                throw new ForceException(errorResponse.error, errorResponse.error_description);
            }
        }

        public async Task<T> QueryById<T>(string objectName, string recordId)
        {
            var fields = string.Join(", ", typeof(T).GetProperties().Select(p => p.Name));
            var query = string.Format("SELECT {0} FROM {1} WHERE Id = '{2}'", fields, objectName, recordId);
            var results = await Query<T>(query);

            return results.FirstOrDefault();
        }

        public async Task<List<SObject>> GetObjects()
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

                    var r = JsonConvert.DeserializeObject<List<SObject>>(jToken.ToString());
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
