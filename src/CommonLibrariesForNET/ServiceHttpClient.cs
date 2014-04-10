﻿﻿//TODO: add license header

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Salesforce.Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Salesforce.Common
{
    public class ServiceHttpClient : IServiceHttpClient, IDisposable
    {
        private static string _userAgent = "common-libraries-dotnet";
        private readonly string _instanceUrl;
        private readonly string _apiVersion;
        private readonly string _accessToken;
        private HttpClient _httpClient;

        public ServiceHttpClient(string instanceUrl, string apiVersion, string accessToken, HttpClient httpClient)
            : this(instanceUrl, apiVersion, accessToken, _userAgent, httpClient)
        {
        }

        public ServiceHttpClient(string instanceUrl, string apiVersion, string accessToken, string userAgent, HttpClient httpClient)
        {
            _instanceUrl = instanceUrl;
            _apiVersion = apiVersion;
            _accessToken = accessToken;
            _userAgent = userAgent;
            _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(string.Concat(_userAgent, "/", _apiVersion));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        public async Task<T> HttpGetAsync<T>(string urlSuffix)
        {
            var url = Common.FormatUrl(urlSuffix, _instanceUrl, _apiVersion);

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };

            var responseMessage = await _httpClient.SendAsync(request);
            var response = await responseMessage.Content.ReadAsStringAsync();

            if (responseMessage.IsSuccessStatusCode)
            {
                var jToken = JToken.Parse(response);
                if (jToken.Type == JTokenType.Array)
                {
                    var jArray = JArray.Parse(response);

                    var r = JsonConvert.DeserializeObject<T>(jArray.ToString());
                    return r;
                }
                else
                {
                    var jObject = JObject.Parse(response);

                    var r = JsonConvert.DeserializeObject<T>(jObject.ToString());
                    return r;
                }
            }

            var errorResponse = JsonConvert.DeserializeObject<ErrorResponses>(response);
            throw new ForceException(errorResponse[0].errorCode, errorResponse[0].message);
        }

        public async Task<IList<T>> HttpGetAsync<T>(string urlSuffix, string nodeName)
        {
            string next = null;
            string response = null;
            var records = new List<T>();

            var url = Common.FormatUrl(urlSuffix, _instanceUrl, _apiVersion);

            try
            {
                do
                {
                    if (next != null)
                        url = Common.FormatUrl(string.Format("query/{0}", next.Split('/').Last()), _instanceUrl, _apiVersion);

                    var request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(url),
                        Method = HttpMethod.Get
                    };

                    var responseMessage = await _httpClient.SendAsync(request);
                    response = await responseMessage.Content.ReadAsStringAsync();

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var jObject = JObject.Parse(response);
                        var jToken = jObject.GetValue(nodeName);

                        next = (jObject.GetValue("nextRecordsUrl") != null) ? jObject.GetValue("nextRecordsUrl").ToString() : null;
                        records.AddRange(JsonConvert.DeserializeObject<IList<T>>(jToken.ToString()));
                    }
                } while (!string.IsNullOrEmpty(next));

                return (IList<T>)records;
            }
            catch (ForceException)
            {
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponses>(response);
                throw new ForceException(errorResponse[0].errorCode, errorResponse[0].message);
            }
        }

        public async Task<T> HttpPostAsync<T>(object inputObject, string urlSuffix)
        {
            var url = Common.FormatUrl(urlSuffix, _instanceUrl, _apiVersion);

            var json = JsonConvert.SerializeObject(inputObject, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var responseMessage = await _httpClient.PostAsync(url, content);
            var response = await responseMessage.Content.ReadAsStringAsync();

            if (responseMessage.IsSuccessStatusCode)
            {
                var r = JsonConvert.DeserializeObject<T>(response);
                return r;
            }

            var errorResponse = JsonConvert.DeserializeObject<ErrorResponses>(response);
            throw new ForceException(errorResponse[0].errorCode, errorResponse[0].message);
        }

        public async Task<bool> HttpPatchAsync(object inputObject, string urlSuffix)
        {
            var url = Common.FormatUrl(urlSuffix, _instanceUrl, _apiVersion);

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = new HttpMethod("PATCH")
            };

            var json = JsonConvert.SerializeObject(inputObject);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var responseMessage = await _httpClient.SendAsync(request);

            if (responseMessage.IsSuccessStatusCode)
            {
                return true;
            }

            var response = await responseMessage.Content.ReadAsStringAsync();
      
            var errorResponse = JsonConvert.DeserializeObject<ErrorResponses>(response);
            throw new ForceException(errorResponse[0].errorCode, errorResponse[0].message);
        }

        public async Task<bool> HttpDeleteAsync(string urlSuffix)
        {
            var url = Common.FormatUrl(urlSuffix, _instanceUrl, _apiVersion);

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Delete
            };

            var responseMessage = await _httpClient.SendAsync(request);

            if (responseMessage.IsSuccessStatusCode)
            {
                return true;
            }

            var response = await responseMessage.Content.ReadAsStringAsync();

            var errorResponse = JsonConvert.DeserializeObject<ErrorResponses>(response);
            throw new ForceException(errorResponse[0].errorCode, errorResponse[0].message);
        }
    }
}
