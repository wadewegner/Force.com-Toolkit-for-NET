using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Salesforce.Common;
using Salesforce.Force.Bulk.Models;

namespace Salesforce.Force.Bulk
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

        public async Task<List<BatchInfoResult>> RunJob(string objectName, Bulk.OperationType operationType,
            IEnumerable<ISObjectList> recordsLists)
        {
            var jobInfoResult = await CreateJobAsync(objectName, operationType);
            var batchResults = new List<BatchInfoResult>();
            foreach (var recordList in recordsLists)
            {
                batchResults.Add(await CreateJobBatchAsync(jobInfoResult, recordList));
            }
            await CloseJobAsync(jobInfoResult);
            return batchResults;
        }

        public async Task<List<BatchResultList>> RunJobAndPoll(string objectName, Bulk.OperationType operationType,
            IEnumerable<ISObjectList> recordsLists)
        {
            const float pollingStart = 1000;
            const float pollingIncrease = 1.6f;

            var batchInfoResults = await RunJob(objectName, operationType, recordsLists);

            var batchResults = new List<BatchResultList>();
            var currentPoll = (int) pollingStart;
            while (batchInfoResults.Count > 0)
            {
                foreach (var batchInfoResult in batchInfoResults.Where(batchInfoResult => batchInfoResult.State == Bulk.BatchState.Completed.Value() ||
                                                                                          batchInfoResult.State == Bulk.BatchState.Failed.Value() ||
                                                                                          batchInfoResult.State == Bulk.BatchState.NotProcessed.Value()))
                {
                    await Task.Delay(currentPoll);
                    batchResults.Add(await GetBatchResult(batchInfoResult));
                    batchInfoResults.Remove(batchInfoResult);
                    currentPoll = (int) (currentPoll * pollingIncrease);
                }
            }

            return batchResults;
        }

        public async Task<JobInfoResult> CreateJobAsync(string objectName, Bulk.OperationType operationType)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");

            var jobInfo = new JobInfo
            {
                ContentType = "XML",
                Object = objectName,
                Operation = operationType.Value()
            };

            return await _bulkServiceHttpClient.HttpPostXmlAsync<JobInfoResult>(jobInfo, "/services/async/{0}/job");
        }

        public async Task<BatchInfoResult> CreateJobBatchAsync(JobInfoResult jobInfo, ISObjectList recordsList)
        {
            return await CreateJobBatchAsync(jobInfo.Id, recordsList).ConfigureAwait(false);
        }

        public async Task<BatchInfoResult> CreateJobBatchAsync(string jobId, ISObjectList recordsObject)
        {
            if (string.IsNullOrEmpty(jobId)) throw new ArgumentNullException("jobId");

            return await _bulkServiceHttpClient.HttpPostXmlAsync<BatchInfoResult>(recordsObject, string.Format("/services/async/{{0}}/job/{0}/batch", jobId))
                .ConfigureAwait(false);
        }

        public async Task<JobInfoResult> CloseJobAsync(JobInfoResult jobInfo)
        {
            return await CloseJobAsync(jobInfo.Id);
        }

        public async Task<JobInfoResult> CloseJobAsync(string jobId)
        {
            if (string.IsNullOrEmpty(jobId)) throw new ArgumentNullException("jobId");

            var state = new JobInfoState {State = "Closed"};
            return await _bulkServiceHttpClient.HttpPostXmlAsync<JobInfoResult>(state, string.Format("/services/async/{{0}}/job/{0}", jobId))
                .ConfigureAwait(false);
        }

        public async Task<JobInfoResult> PollJobAsync(JobInfoResult jobInfo)
        {
            return await PollJobAsync(jobInfo.Id);
        }

        public async Task<JobInfoResult> PollJobAsync(string jobId)
        {
            if (string.IsNullOrEmpty(jobId)) throw new ArgumentNullException("jobId");

            return await _bulkServiceHttpClient.HttpGetXmlAsync<JobInfoResult>(string.Format("/services/async/{{0}}/job/{0}", jobId))
                .ConfigureAwait(false);
        }

        public async Task<BatchInfoResult> PollBatchAsync(BatchInfoResult batchInfo)
        {
            return await PollBatchAsync(batchInfo.Id, batchInfo.JobId);
        }

        public async Task<BatchInfoResult> PollBatchAsync(string batchId, string jobId)
        {
            if (string.IsNullOrEmpty(batchId)) throw new ArgumentNullException("batchId");
            if (string.IsNullOrEmpty(jobId)) throw new ArgumentNullException("jobId");

            return await _bulkServiceHttpClient.HttpGetXmlAsync<BatchInfoResult>(string.Format("/services/async/{{0}}/job/{0}/batch/{1}", jobId, batchId))
                .ConfigureAwait(false);
        }

        public async Task<BatchResultList> GetBatchResult(BatchInfoResult batchInfo)
        {
            return await GetBatchResult(batchInfo.Id, batchInfo.JobId);
        }

        public async Task<BatchResultList> GetBatchResult(string batchId, string jobId)
        {
            if (string.IsNullOrEmpty(batchId)) throw new ArgumentNullException("batchId");
            if (string.IsNullOrEmpty(jobId)) throw new ArgumentNullException("jobId");

            return await _bulkServiceHttpClient.HttpGetXmlAsync<BatchResultList>(string.Format("/services/async/{{0}}/job/{0}/batch/{1}/result", jobId, batchId))
               .ConfigureAwait(false);
        }

        public void Dispose()
        {
            _bulkServiceHttpClient.Dispose();
        }
    }
}
