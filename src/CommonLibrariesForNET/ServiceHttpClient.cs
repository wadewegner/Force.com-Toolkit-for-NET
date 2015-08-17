﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Salesforce.Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Salesforce.Common.Serializer;

namespace Salesforce.Common
{
    public class ServiceHttpClient : IServiceHttpClient, IDisposable
    {
        private const string UserAgent = "forcedotcom-toolkit-dotnet";
        private readonly string _instanceUrl;
        private readonly string _apiVersion;
        private readonly HttpClient _httpClient;

        public ServiceHttpClient(string instanceUrl, string apiVersion, string accessToken, HttpClient httpClient)
        {
            _instanceUrl = instanceUrl;
            _apiVersion = apiVersion;
            _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(string.Concat(UserAgent, "/", _apiVersion));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        // With standard GET and ApexRest GET we can factor out the message send and response processing
        // using this function.
        public async Task<T> HttpGetAsync<T>(HttpRequestMessage request)
        {
            var responseMessage = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

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
            throw new ForceException(errorResponse[0].ErrorCode, errorResponse[0].Message);
        }
        
        public async Task<T> HttpGetAsync<T>(Uri url)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = url,
                Method = HttpMethod.Get
            };

            return await HttpGetAsync<T>(request);
        }

        public async Task<T> HttpGetAsync<T>(string urlSuffix, string parameters=null)
        {
            var url = Common.FormatUrl(urlSuffix, _instanceUrl, _apiVersion, parameters);

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };

            return await HttpGetAsync<T>(request);
        }

		/// <summary>
        /// GET from a custom REST API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiName">The name of the custom REST API</param>
        /// <param name="parameters">Pre-formatted parameters like this: ?name1=value1&name2=value2&soon=soforth</param>
        /// <returns></returns>
        public async Task<T> HttpGetApexRestAsync<T>(string apiName, string parameters)
        {
            var url = new Uri(Common.FormatCustomUrl(apiName, parameters, _instanceUrl));

            return await HttpGetAsync<T>(url);
        }
        /// <summary>
        /// POST to a custom REST API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiName">The name of the custom REST API</param>
        /// <param name="parameters">JSON formatted string</param>
        /// <returns></returns>
        public async Task<T> HttpPostApexRestAsync<T>(string apiName, object parameters)
        {
            var url = new Uri(Common.FormatCustomUrl(apiName, "", _instanceUrl));

            return await HttpPostAsync<T>(parameters, url);
        }
        
        public async Task<T> HttpPostAsync<T>(object inputObject, string urlSuffix)
        {
            var url = Common.FormatUrl(urlSuffix, _instanceUrl, _apiVersion);
            var json = JsonConvert.SerializeObject(inputObject,
                Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CreateableContractResolver()
                });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var responseMessage = await _httpClient.PostAsync(new Uri(url), content).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode)
            {
                var r = JsonConvert.DeserializeObject<T>(response);
                return r;
            }

            var errorResponse = JsonConvert.DeserializeObject<ErrorResponses>(response);
            throw new ForceException(errorResponse[0].ErrorCode, errorResponse[0].Message);
        }

        public async Task<T> HttpPostAsync<T>(object inputObject, Uri uri)
        {
            var json = JsonConvert.SerializeObject(inputObject,
                Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                });
            return await HttpPostAsync<T>(json, uri);
        }

        public async Task<T> HttpPostAsync<T>(string json, Uri uri) {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var responseMessage = await _httpClient.PostAsync(uri, content).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode)
            {
                var r = JsonConvert.DeserializeObject<T>(response);
                return r;
            }

            var errorResponse = JsonConvert.DeserializeObject<ErrorResponses>(response);
            throw new ForceException(errorResponse[0].ErrorCode, errorResponse[0].Message);
        }

        public async Task<SuccessResponse> HttpPatchAsync(object inputObject, string urlSuffix)
        {
            var url = Common.FormatUrl(urlSuffix, _instanceUrl, _apiVersion);

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = new HttpMethod("PATCH")
            };

            var json = JsonConvert.SerializeObject(inputObject,
                Formatting.None,
                new JsonSerializerSettings
                {
                    ContractResolver = new UpdateableContractResolver()
                });

            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var responseMessage = await _httpClient.SendAsync(request).ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode) {
                if (responseMessage.StatusCode != System.Net.HttpStatusCode.NoContent) {
                    var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                    var r = JsonConvert.DeserializeObject<SuccessResponse>(response);
                    return r;
                }

                var success = new SuccessResponse { Id = "", Errors = "", Success = "true" };
                return success;
            }

            var error = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            var errorResponse = JsonConvert.DeserializeObject<ErrorResponses>(error);
            throw new ForceException(errorResponse[0].ErrorCode, errorResponse[0].Message);
        }

        public async Task<bool> HttpDeleteAsync(string urlSuffix)
        {
            var url = Common.FormatUrl(urlSuffix, _instanceUrl, _apiVersion);

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Delete
            };

            var responseMessage = await _httpClient.SendAsync(request).ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode)
            {
                return true;
            }

            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            var errorResponse = JsonConvert.DeserializeObject<ErrorResponses>(response);
            throw new ForceException(errorResponse[0].ErrorCode, errorResponse[0].Message);
        }
    }
}
