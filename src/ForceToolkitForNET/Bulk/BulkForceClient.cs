using System;
using System.Collections.Generic;
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

        public async Task<List<BatchInfoResult>> RunJob<T>(string objectName, Bulk.OperationType operationType,
            IEnumerable<ISObjectList<T>> recordsLists)
        {
            if (recordsLists == null) throw new ArgumentNullException("recordsLists");

            var jobInfoResult = await CreateJobAsync(objectName, operationType);
            var batchResults = new List<BatchInfoResult>();
            foreach (var recordList in recordsLists)
            {
                batchResults.Add(await CreateJobBatchAsync(jobInfoResult, recordList));
            }
            await CloseJobAsync(jobInfoResult);
            return batchResults;
        }

        public async Task<List<BatchResultList>> RunJobAndPoll<T>(string objectName, Bulk.OperationType operationType,
            IEnumerable<ISObjectList<T>> recordsLists)
        {
            const float pollingStart = 1000;
            const float pollingIncrease = 2.0f;

            var batchInfoResults = await RunJob(objectName, operationType, recordsLists);

            var batchResults = new List<BatchResultList>();
            var currentPoll = pollingStart;
            while (batchInfoResults.Count > 0)
            {
                var removeList = new List<BatchInfoResult>();
                foreach (var batchInfoResult in batchInfoResults)
                {
                    var batchInfoResultNew = await PollBatchAsync(batchInfoResult);
                    if (batchInfoResultNew.State.Equals(Bulk.BatchState.Completed.Value()) ||
                        batchInfoResultNew.State.Equals(Bulk.BatchState.Failed.Value()) ||
                        batchInfoResultNew.State.Equals(Bulk.BatchState.NotProcessed.Value()))
                    {
                        batchResults.Add(await GetBatchResult(batchInfoResultNew));
                        removeList.Add(batchInfoResult);
                    }
                }
                foreach (var removeItem in removeList)
                {
                    batchInfoResults.Remove(removeItem);
                }

                await Task.Delay((int)currentPoll);
                currentPoll *= pollingIncrease;
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

        public async Task<BatchInfoResult> CreateJobBatchAsync<T>(JobInfoResult jobInfo, ISObjectList<T> recordsList)
        {
            if (jobInfo == null) throw new ArgumentNullException("jobInfo");
            return await CreateJobBatchAsync(jobInfo.Id, recordsList).ConfigureAwait(false);
        }

        public async Task<BatchInfoResult> CreateJobBatchAsync<T>(string jobId, ISObjectList<T> recordsObject)
        {
            if (string.IsNullOrEmpty(jobId)) throw new ArgumentNullException("jobId");
            if (recordsObject == null) throw new ArgumentNullException("recordsObject");

            return await _bulkServiceHttpClient.HttpPostXmlAsync<BatchInfoResult>(recordsObject, string.Format("/services/async/{{0}}/job/{0}/batch", jobId))
                .ConfigureAwait(false);
        }

        public async Task<JobInfoResult> CloseJobAsync(JobInfoResult jobInfo)
        {
            if (jobInfo == null) throw new ArgumentNullException("jobInfo");
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
            if (jobInfo == null) throw new ArgumentNullException("jobInfo");
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
            if (batchInfo == null) throw new ArgumentNullException("batchInfo");
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
            if (batchInfo == null) throw new ArgumentNullException("batchInfo");
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
