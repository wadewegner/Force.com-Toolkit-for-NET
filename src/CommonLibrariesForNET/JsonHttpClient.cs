using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Salesforce.Common.Models.Json;
using Salesforce.Common.Serializer;

namespace Salesforce.Common
{
    public class JsonHttpClient : BaseHttpClient, IJsonHttpClient
    {

        public JsonHttpClient(string instanceUrl, string apiVersion, string accessToken, HttpClient httpClient)
            : base(instanceUrl, apiVersion, "application/json", httpClient)
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        private static ForceException ParseForceException(string responseMessage)
        {
            var errorResponse = JsonConvert.DeserializeObject<ErrorResponses>(responseMessage);
            return new ForceException(errorResponse[0].ErrorCode, errorResponse[0].Message);
        }

        // GET

        public async Task<T> HttpGetAsync<T>(string urlSuffix)
        {
            var url = Common.FormatUrl(urlSuffix, InstanceUrl, ApiVersion);
            return await HttpGetAsync<T>(new Uri(url));
        }

        public async Task<T> HttpGetAsync<T>(Uri uri)
        {
            try
            {
                var response = await HttpGetAsync(uri);
                var jToken = JToken.Parse(response);
                if (jToken.Type == JTokenType.Array)
                {
                    var jArray = JArray.Parse(response);
                    return JsonConvert.DeserializeObject<T>(jArray.ToString());
                }
                // else
                var jObject = JObject.Parse(response);
                return JsonConvert.DeserializeObject<T>(jObject.ToString());
            }
            catch (BaseHttpClientException e)
            {
                throw ParseForceException(e.Message);
            }
        }

        public async Task<IList<T>> HttpGetAsync<T>(string urlSuffix, string nodeName)
        {
            string next = null;
            var records = new List<T>();
            var url = Common.FormatUrl(urlSuffix, InstanceUrl, ApiVersion);

            do
            {
                if (next != null)
                {
                    url = Common.FormatUrl(string.Format("query/{0}", next.Split('/').Last()), InstanceUrl, ApiVersion);
                }
                try
                {
                    var response = await HttpGetAsync(new Uri(url));
                    var jObject = JObject.Parse(response);
                    var jToken = jObject.GetValue(nodeName);
                    next = (jObject.GetValue("nextRecordsUrl") != null) ? jObject.GetValue("nextRecordsUrl").ToString() : null;
                    records.AddRange(JsonConvert.DeserializeObject<IList<T>>(jToken.ToString()));
                }
                catch (BaseHttpClientException e)
                {
                    throw ParseForceException(e.Message);
                }
            }
            while (!string.IsNullOrEmpty(next));

            return records;
        }

		/// <summary>
        /// Call a custom REST API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiName">The name of the custom REST API</param>
        /// <param name="parameters">Pre-formatted parameters like this: ?name1=value1&name2=value2&soon=soforth</param>
        /// <returns></returns>
        public async Task<T> HttpGetRestApiAsync<T>(string apiName, string parameters)
        {
            var url = Common.FormatCustomUrl(apiName, parameters, InstanceUrl);
            return await HttpGetAsync<T>(url);
        }

        // POST

        public async Task<T> HttpPostAsync<T>(object inputObject, string urlSuffix)
        {
            var url = Common.FormatUrl(urlSuffix, InstanceUrl, ApiVersion);
            return await HttpPostAsync<T>(inputObject, new Uri(url));
        }

        public async Task<T> HttpPostAsync<T>(object inputObject, Uri uri)
        {
            var json = JsonConvert.SerializeObject(inputObject,
                   Formatting.None,
                   new JsonSerializerSettings
                   {
                       NullValueHandling = NullValueHandling.Ignore,
                       ContractResolver = new CreateableContractResolver()
                   });
            try
            {
                var response = await HttpPostAsync(json, uri);
                return JsonConvert.DeserializeObject<T>(response);
            }
            catch (BaseHttpClientException e)
            {
                throw ParseForceException(e.Message);
            }
        }

        // PATCH

        public async Task<SuccessResponse> HttpPatchAsync(object inputObject, string urlSuffix)
        {
            var url = Common.FormatUrl(urlSuffix, InstanceUrl, ApiVersion);
            return await HttpPatchAsync(inputObject, new Uri(url));
        }

        public async Task<SuccessResponse> HttpPatchAsync(object inputObject, Uri uri)
        {
            var json = JsonConvert.SerializeObject(inputObject,
                Formatting.None,
                new JsonSerializerSettings
                {
                    ContractResolver = new UpdateableContractResolver()
                });
            try
            {
                var response = await base.HttpPatchAsync(json, uri);
                return string.IsNullOrEmpty(response) ?
                    new SuccessResponse{ Id = "", Errors = "", Success = "true" } :
                    JsonConvert.DeserializeObject<SuccessResponse>(response);
            }
            catch (BaseHttpClientException e)
            {
                throw ParseForceException(e.Message);
            }
        }

        // DELETE

        public async Task<bool> HttpDeleteAsync(string urlSuffix)
        {
            var url = Common.FormatUrl(urlSuffix, InstanceUrl, ApiVersion);
            return await HttpDeleteAsync(new Uri(url));
        }

        public new async Task<bool> HttpDeleteAsync(Uri uri)
        {
            try
            {
                await base.HttpDeleteAsync(uri);
                return true;
            }
            catch (BaseHttpClientException e)
            {
                throw ParseForceException(e.Message);
            }
        }
    }
}
