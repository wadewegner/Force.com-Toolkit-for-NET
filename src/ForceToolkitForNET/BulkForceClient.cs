using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Salesforce.Common;
using Salesforce.Common.Models.Xml;
using Salesforce.Common.Serializer;

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

            return await _bulkServiceHttpClient.HttpPostXmlAsync<JobInfoResult>(jobInfo, "/services/async/{0}/job");
        }

        public async Task<BatchInfoResult> CreateJobBatchAsync<T>(JobInfoResult jobInfo, List<T> recordsList)
        {
            return await CreateJobBatchAsync(jobInfo.Id, recordsList).ConfigureAwait(false);
        }

        public async Task<BatchInfoResult> CreateJobBatchAsync<T>(string jobId, List<T> recordList)
        {
            var recordListCsv = await CsvSerializer.SerializeList(recordList);
            return await CreateJobBatchAsync(jobId, recordListCsv).ConfigureAwait(false);
        }

        public async Task<BatchInfoResult> CreateJobBatchAsync(JobInfoResult jobInfo, string csvData)
        {
            return await CreateJobBatchAsync(jobInfo.Id, csvData).ConfigureAwait(false);
        }

        public async Task<BatchInfoResult> CreateJobBatchAsync(string jobId, string csvData)
        {
            return await _bulkServiceHttpClient.HttpPostCsvAsync<BatchInfoResult>(csvData, string.Format("/services/async/{{0}}/job/{0}/batch", jobId))
                .ConfigureAwait(false);
        }

        public async Task<JobInfoResult> CloseJobAsync(JobInfoResult jobInfo)
        {
            return await CloseJobAsync(jobInfo.Id);
        }

        public async Task<JobInfoResult> CloseJobAsync(string jobId)
        {
            var state = new JobInfoState {State = "Closed"};
            return await _bulkServiceHttpClient.HttpPostXmlAsync<JobInfoResult>(state, string.Format("/services/async/{{0}}/job/{0}", jobId))
                .ConfigureAwait(false);
        }

        public void Dispose()
        {
            _bulkServiceHttpClient.Dispose();
        }
    }
}
