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
using Salesforce.Common.Serializer;

namespace Salesforce.Common
{
    public class ServiceHttpClient: IServiceHttpClient, IDisposable
    {
        private const string UserAgent = "forcedotcom-toolkit-dotnet";
        private const string DateFormat = "s";
        private readonly string _instanceUrl;
        public string ApiVersion;
        private HttpClient _httpClient;
        private readonly bool _disposeHttpClient;

        public ServiceHttpClient(string instanceUrl, string apiVersion, string accessToken, HttpClient httpClient = null)
        {
            _instanceUrl = instanceUrl;
            ApiVersion = apiVersion;

            if (httpClient == null)
            {
                _httpClient = new HttpClient();
                _disposeHttpClient = true;
            }
            else
            { 
                _httpClient = httpClient;
            }
            
            _httpClient.DefaultRequestHeaders.UserAgent.Clear();
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(UserAgent, ApiVersion));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        }

        public void Dispose()
        {
            if (_disposeHttpClient)
            {
                _httpClient.Dispose();
            }
            _httpClient = null;
        }

        public async Task<T> HttpGetAsync<T>(string urlSuffix)
        {
            var uri = Common.FormatUrl(urlSuffix, _instanceUrl, ApiVersion);

            var request = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = HttpMethod.Get
            };

            var responseMessage = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsDecompressedStringAsync().ConfigureAwait(false);

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

            if (responseMessage.Content.Headers.ContentType != null && responseMessage.Content.Headers.ContentType.ToString().Contains("application/json"))
            {
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponses>(response);
                throw new ForceException(errorResponse[0].ErrorCode, errorResponse[0].Message);
            }
            else
            {
                throw new ForceException(Error.NonJsonErrorResponse, response);
            }
        }

        public async Task<T> HttpGetRestApiAsync<T>(string apiName)
        {
            var url = Common.FormatRestApiUrl(apiName, _instanceUrl);

            return await HttpGetAsync<T>(url);
        }

        public async Task<IList<T>> HttpGetAsync<T>(string urlSuffix, string nodeName)
        {
            string next = null;
            string response = null;
            var records = new List<T>();

            var uri = Common.FormatUrl(urlSuffix, _instanceUrl, ApiVersion);

            try
            {
                do
                {
                    if (next != null)
                        uri = Common.FormatUrl(string.Format("query/{0}", next.Split('/').Last()), _instanceUrl, ApiVersion);

                    var request = new HttpRequestMessage
                    {
                        RequestUri = uri,
                        Method = HttpMethod.Get
                    };

                    var responseMessage = await _httpClient.SendAsync(request).ConfigureAwait(false);
                    response = await responseMessage.Content.ReadAsDecompressedStringAsync().ConfigureAwait(false);

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
                throw new ForceException(errorResponse[0].ErrorCode, errorResponse[0].Message);
            }
        }

        public async Task<T> HttpGetAsync<T>(Uri uri)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = HttpMethod.Get
            };

            var responseMessage = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsDecompressedStringAsync().ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode)
            {
                var r = JsonConvert.DeserializeObject<T>(response);
                return r;
            }

            if (responseMessage.Content.Headers.ContentType != null && responseMessage.Content.Headers.ContentType.ToString().Contains("application/json"))
            {
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponses>(response);
                throw new ForceException(errorResponse[0].ErrorCode, errorResponse[0].Message);
            }
            else
            {
                throw new ForceException(Error.NonJsonErrorResponse, response);
            }

        }

        public async Task<T> HttpPostRestApiAsync<T>(string apiName, object inputObject)
        {
            var url = Common.FormatRestApiUrl(apiName, _instanceUrl);

            return await HttpPostAsync<T>(inputObject, url);
        }

        public async Task<T> HttpPostAsync<T>(object inputObject, string urlSuffix)
        {
            var uri = Common.FormatUrl(urlSuffix, _instanceUrl, ApiVersion);
            var json = JsonConvert.SerializeObject(inputObject,
                Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CreateableContractResolver(),
                    DateFormatString = DateFormat
                });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var responseMessage = await _httpClient.PostAsync(uri, content).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsDecompressedStringAsync().ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode)
            {
                var r = JsonConvert.DeserializeObject<T>(response);
                return r;
            }

            if (responseMessage.Content.Headers.ContentType != null && responseMessage.Content.Headers.ContentType.ToString().Contains("application/json"))
            {
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponses>(response);
                throw new ForceException(errorResponse[0].ErrorCode, errorResponse[0].Message);
            }
            else
            {
                throw new ForceException(Error.NonJsonErrorResponse, response);
            }

        }

        public async Task<T> HttpPostAsync<T>(object inputObject, Uri uri)
        {
            var json = JsonConvert.SerializeObject(inputObject,
                Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatString = DateFormat
                });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var responseMessage = await _httpClient.PostAsync(uri, content).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsDecompressedStringAsync().ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode)
            {
                var r = JsonConvert.DeserializeObject<T>(response);
                return r;
            }

            if (responseMessage.Content.Headers.ContentType != null && responseMessage.Content.Headers.ContentType.ToString().Contains("application/json"))
            {
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponses>(response);
                throw new ForceException(errorResponse[0].ErrorCode, errorResponse[0].Message);
            }
            else
            {
                throw new ForceException(Error.NonJsonErrorResponse, response);
            }

        }

        public async Task<SuccessResponse> HttpPatchAsync(object inputObject, string urlSuffix)
        {
            var uri = Common.FormatUrl(urlSuffix, _instanceUrl, ApiVersion);

            var request = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = new HttpMethod("PATCH")
            };

            var json = JsonConvert.SerializeObject(inputObject,
                Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new UpdateableContractResolver(),
                    DateFormatString = DateFormat
                });

            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var responseMessage = await _httpClient.SendAsync(request).ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode)
            {
                if (responseMessage.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    var response = await responseMessage.Content.ReadAsDecompressedStringAsync().ConfigureAwait(false);

                    var r = JsonConvert.DeserializeObject<SuccessResponse>(response);
                    return r;
                }

                var success = new SuccessResponse { Id = "", Errors = "", Success = true };
                return success;
            }

            var error = await responseMessage.Content.ReadAsDecompressedStringAsync().ConfigureAwait(false);
            if (responseMessage.Content.Headers.ContentType != null && responseMessage.Content.Headers.ContentType.ToString().Contains("application/json"))
            {
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponses>(error);
                throw new ForceException(errorResponse[0].ErrorCode, errorResponse[0].Message);
            }
            else
            {
                throw new ForceException(Error.NonJsonErrorResponse, error);
            }

        }

        public async Task<bool> HttpDeleteAsync(string urlSuffix)
        {
            var uri = Common.FormatUrl(urlSuffix, _instanceUrl, ApiVersion);

            var request = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = HttpMethod.Delete
            };

            var responseMessage = await _httpClient.SendAsync(request).ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode)
            {
                return true;
            }

            var response = await responseMessage.Content.ReadAsDecompressedStringAsync().ConfigureAwait(false);

            if (responseMessage.Content.Headers.ContentType != null && responseMessage.Content.Headers.ContentType.ToString().Contains("application/json"))
            {
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponses>(response);
                throw new ForceException(errorResponse[0].ErrorCode, errorResponse[0].Message);
            }
            else
            {
                throw new ForceException(Error.NonJsonErrorResponse, response);
            }
        }

        public async Task<T> HttpBinaryDataPostAsync<T>(string urlSuffix, object inputObject, byte[] fileContents, string headerName, string fileName)
        {
            var uri = Common.FormatUrl(urlSuffix, _instanceUrl, ApiVersion);

            var json = JsonConvert.SerializeObject(inputObject,
                Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                });

            var content = new MultipartFormDataContent();

            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            stringContent.Headers.Add("Content-Disposition", "form-data; name=\"json\"");
            content.Add(stringContent);

            var byteArrayContent = new ByteArrayContent(fileContents);
            byteArrayContent.Headers.Add("Content-Type", "application/octet-stream");
            byteArrayContent.Headers.Add("Content-Disposition", String.Format("form-data; name=\"{0}\"; filename=\"{1}\"", headerName, fileName));
            content.Add(byteArrayContent, headerName, fileName);

            var responseMessage = await _httpClient.PostAsync(uri, content).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsDecompressedStringAsync().ConfigureAwait(false);

            if (responseMessage.IsSuccessStatusCode)
            {
                var r = JsonConvert.DeserializeObject<T>(response);
                return r;
            }

            if (responseMessage.Content.Headers.ContentType != null && responseMessage.Content.Headers.ContentType.ToString().Contains("application/json"))
            {
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponses>(response);
                throw new ForceException(errorResponse[0].ErrorCode, errorResponse[0].Message);
            }
            else
            {
                throw new ForceException(Error.NonJsonErrorResponse, response);
            }
        }
    }
}
