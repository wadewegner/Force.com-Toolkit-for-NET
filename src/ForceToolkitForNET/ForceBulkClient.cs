using System;
using System.Net.Http;
using Salesforce.Common;

namespace Salesforce.Force
{
    public class ForceBulkClient : IForceBulkClient, IDisposable
    {
        private readonly SoapServiceHttpClient _soapServiceHttpClient;

        public ForceBulkClient(string instanceUrl, string accessToken, string apiVersion)
            : this(instanceUrl, accessToken, apiVersion, new HttpClient())
        {
        }

        public ForceBulkClient(string instanceUrl, string accessToken, string apiVersion, HttpClient httpClient)
        {
            if (string.IsNullOrEmpty(instanceUrl)) throw new ArgumentNullException("instanceUrl");
            if (string.IsNullOrEmpty(accessToken)) throw new ArgumentNullException("accessToken");
            if (string.IsNullOrEmpty(apiVersion)) throw new ArgumentNullException("apiVersion");
            if (httpClient == null) throw new ArgumentNullException("httpClient");

            _soapServiceHttpClient = new SoapServiceHttpClient(instanceUrl, apiVersion, accessToken, httpClient);
        }


        public void Dispose()
        {
            _soapServiceHttpClient.Dispose();
        }
    }
}
