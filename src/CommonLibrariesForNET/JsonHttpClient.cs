using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Salesforce.Common.Internals;
using Salesforce.Common.Models.Json;
using Salesforce.Common.Serializer;

namespace Salesforce.Common
{
    public class JsonHttpClient : BaseHttpClient, IJsonHttpClient
    {
        private const string DateFormat = "s";

        public JsonHttpClient(string instanceUrl, string apiVersion, string accessToken, HttpClient httpClient, bool callerWillDisposeHttpClient = false)
            : base(instanceUrl, apiVersion, "application/json", httpClient, callerWillDisposeHttpClient)
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        private static ForceException ParseForceException(string responseMessage)
        {
            ErrorResponses errorResponse;
            
            try
            {
                errorResponse = JsonConvert.DeserializeObject<ErrorResponses>(responseMessage);
            }
            catch (Exception e)
            {
                // return initial response if can't parse
                // <h1>Bad Message 414</h1><pre>reason: URI Too Long</pre>
                return new ForceException(Error.Unknown, responseMessage + Environment.NewLine + e.Message);
            }
            
            return new ForceException(errorResponse[0].ErrorCode, errorResponse[0].Message);
        }

        // GET

        public async Task<T> HttpGetAsync<T>(string urlSuffix)
        {
            var url = Common.FormatUrl(urlSuffix, InstanceUrl, ApiVersion);
            return await HttpGetAsync<T>(url);
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
                try
                {
                    var jObject = JObject.Parse(response);
                    return JsonConvert.DeserializeObject<T>(jObject.ToString());
                }
                catch
                {
                    return JsonConvert.DeserializeObject<T>(response);
                }
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
                    var response = await HttpGetAsync(url);
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


        public async Task<System.IO.Stream> HttpGetBlobAsync(string urlSuffix)
        {
            var url = Common.FormatUrl(urlSuffix, InstanceUrl, ApiVersion);

            var request = new HttpRequestMessage
            {
                RequestUri = url,
                Method = HttpMethod.Get
            };

            var responseMessage = await HttpClient.SendAsync(request).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsStreamAsync();

            if (responseMessage.IsSuccessStatusCode)
            {
                return response;
            }
            else
            {
                return new System.IO.MemoryStream();
            }
        }

        public async Task<T> HttpGetRestApiAsync<T>(string apiName)
        {
            var url = Common.FormatRestApiUrl(apiName, InstanceUrl);
            return await HttpGetAsync<T>(url);
        }

        // POST

        public async Task<T> HttpPostAsync<T>(object inputObject, string urlSuffix)
        {
            var url = Common.FormatUrl(urlSuffix, InstanceUrl, ApiVersion);
            return await HttpPostAsync<T>(inputObject, url);
        }

        public async Task<T> HttpPostAsync<T>(object inputObject, Uri uri)
        {
            var json = JsonConvert.SerializeObject(inputObject,
                   Formatting.None,
                   new JsonSerializerSettings
                   {
                       NullValueHandling = NullValueHandling.Ignore,
                       ContractResolver = new CreateableContractResolver(),
                       DateFormatString = DateFormat
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

        public async Task<T> HttpPostRestApiAsync<T>(string apiName, object inputObject)
        {
            var url = Common.FormatRestApiUrl(apiName, InstanceUrl);
            return await HttpPostAsync<T>(inputObject, url);
        }

        public async Task<T> HttpBinaryDataPostAsync<T>(string urlSuffix, object inputObject, byte[] fileContents, string headerName, string fileName)
        {
            // BRAD: I think we should probably, in time, refactor multipart and binary support to the BaseHttpClient.
            // For now though, I just left this in here.

            var uri = Common.FormatUrl(urlSuffix, InstanceUrl, ApiVersion);

            var json = JsonConvert.SerializeObject(inputObject,
                Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            var content = new MultipartFormDataContent();

            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            stringContent.Headers.Add("Content-Disposition", "form-data; name=\"json\"");
            content.Add(stringContent);

            var byteArrayContent = new ByteArrayContent(fileContents);
            byteArrayContent.Headers.Add("Content-Type", "application/octet-stream");
            byteArrayContent.Headers.Add("Content-Disposition", string.Format("form-data; name=\"{0}\"; filename=\"{1}\"", headerName, fileName));
            content.Add(byteArrayContent, headerName, fileName);

            var responseMessage = await HttpClient.PostAsync(uri, content).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(response);
            }

            throw ParseForceException(response);
        }

        // PATCH

        public async Task<SuccessResponse> HttpPatchAsync(object inputObject, string urlSuffix)
        {
            var url = Common.FormatUrl(urlSuffix, InstanceUrl, ApiVersion);
            return await HttpPatchAsync(inputObject, url);
        }

        public async Task<SuccessResponse> HttpPatchAsync(object inputObject, string urlSuffix, bool ignoreNull)
        {
            var url = Common.FormatUrl(urlSuffix, InstanceUrl, ApiVersion);
            if (ignoreNull == true)
            {
                return await HttpPatchAsync(inputObject, url);
            }
            else
            {
                return await HttpPatchAsync(inputObject, url, NullValueHandling.Include);
            }
         //   return await HttpPatchAsync(inputObject, url, ignoreNull);
        }


        public async Task<SuccessResponse> HttpPatchAsync(object inputObject, Uri uri)
        {
            var json = JsonConvert.SerializeObject(inputObject,
                Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new UpdateableContractResolver(),
                    DateFormatString = DateFormat
                });
            try
            {
                var response = await base.HttpPatchAsync(json, uri);
                return string.IsNullOrEmpty(response) ?
                    new SuccessResponse{ Id = "", Errors = "", Success = true } :
                    JsonConvert.DeserializeObject<SuccessResponse>(response);
            }
            catch (BaseHttpClientException e)
            {
                throw ParseForceException(e.Message);
            }
        }

        public async Task<SuccessResponse> HttpPatchAsync(object inputObject, Uri uri, NullValueHandling nullValueHandling )
        {
            
                var json = JsonConvert.SerializeObject(inputObject,
                              Formatting.None,
                              new JsonSerializerSettings
                              {
                                  NullValueHandling = nullValueHandling,
                                  ContractResolver = new UpdateableContractResolver(),
                                  DateFormatString = DateFormat
                              });
            
           
            try
            {
                var response = await base.HttpPatchAsync(json, uri);
                return string.IsNullOrEmpty(response) ?
                    new SuccessResponse { Id = "", Errors = "", Success = true } :
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
            return await HttpDeleteAsync(url);
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
