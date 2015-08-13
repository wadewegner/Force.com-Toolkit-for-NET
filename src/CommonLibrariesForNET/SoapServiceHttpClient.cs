using System;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public T HttpPostAsync<T>(object postBody)
        {
            SetXmlHeader();
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
