using System;
using System.Net.Http;
using System.Threading.Tasks;
using Salesforce.Common;
using Salesforce.Common.Models.Xml;

namespace Salesforce.Force
{
    public class BulkForceClient : IBulkForceClient, IDisposable
    {
        private readonly BulkServiceHttpClient _bulkServiceHttpClient;

        public BulkForceClient(string instanceUrl, string accessToken, string apiVersion)
            : this(instanceUrl, accessToken, apiVersion, new HttpClient())
        {
        }

        public BulkForceClient(string instanceUrl, string accessToken, string apiVersion, HttpClient httpClient)
        {
            if (string.IsNullOrEmpty(instanceUrl)) throw new ArgumentNullException("instanceUrl");
            if (string.IsNullOrEmpty(accessToken)) throw new ArgumentNullException("accessToken");
            if (string.IsNullOrEmpty(apiVersion)) throw new ArgumentNullException("apiVersion");
            if (httpClient == null) throw new ArgumentNullException("httpClient");

            _bulkServiceHttpClient = new BulkServiceHttpClient(instanceUrl, apiVersion, accessToken, httpClient);
        }

        public async Task<JobInfoResult> CreateJobAsync(string objectName, BulkOperationType operationType)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");

            string opTypeString = null;
            switch (operationType)
            {
                case BulkOperationType.Insert:
                    opTypeString = "insert";
                    break;
            }

            var jobInfo = new JobInfo
            {
                ContentType = "CSV",
                Object = objectName,
                Operation = opTypeString
            };

            return await _bulkServiceHttpClient.HttpPostAsync<JobInfoResult>(jobInfo, "/services/async/{0}/job");
        }

        public void Dispose()
        {
            _bulkServiceHttpClient.Dispose();
        }
    }
}
