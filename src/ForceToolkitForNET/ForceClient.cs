using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;
using Salesforce.Common;
using Salesforce.Common.Models.Json;
using Salesforce.Common.Models.Xml;

namespace Salesforce.Force
{
    public class ForceClient : IForceClient, IDisposable
    {
        private readonly IXmlHttpClient _xmlHttpClient;
        private readonly IJsonHttpClient _jsonHttpClient;

        public ForceClient(string instanceUrl, string accessToken, string apiVersion)
            : this(instanceUrl, accessToken, apiVersion, new HttpClient(), new HttpClient())
        {
        }

        public ForceClient(string instanceUrl, string accessToken, string apiVersion, HttpClient httpClientForJson, HttpClient httpClientForXml)
        {
            if (string.IsNullOrEmpty(instanceUrl)) throw new ArgumentNullException("instanceUrl");
            if (string.IsNullOrEmpty(accessToken)) throw new ArgumentNullException("accessToken");
            if (string.IsNullOrEmpty(apiVersion)) throw new ArgumentNullException("apiVersion");
            if (httpClientForJson == null) throw new ArgumentNullException("httpClientForJson");
            if (httpClientForXml == null) throw new ArgumentNullException("httpClientForXml");

            _jsonHttpClient = new JsonHttpClient(instanceUrl, apiVersion, accessToken, httpClientForJson);
            _xmlHttpClient = new XmlHttpClient(instanceUrl, apiVersion, accessToken, httpClientForXml);
        }

        // STANDARD METHODS

        public Task<QueryResult<T>> QueryAsync<T>(string query, CancellationToken token)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException("query");

            return _jsonHttpClient.HttpGetAsync<QueryResult<T>>(string.Format("query?q={0}", Uri.EscapeDataString(query)), token);
        }

        public Task<QueryResult<T>> QueryContinuationAsync<T>(string nextRecordsUrl, CancellationToken token)
        {
            if (string.IsNullOrEmpty(nextRecordsUrl)) throw new ArgumentNullException("nextRecordsUrl");

            return _jsonHttpClient.HttpGetAsync<QueryResult<T>>(nextRecordsUrl, token);
        }

        public Task<QueryResult<T>> QueryAllAsync<T>(string query, CancellationToken token)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException("query");

            return _jsonHttpClient.HttpGetAsync<QueryResult<T>>(string.Format("queryAll/?q={0}", Uri.EscapeDataString(query)), token);
        }

        public Task<T> ExecuteRestApiAsync<T>(string apiName, CancellationToken token)
        {
            if (string.IsNullOrEmpty(apiName)) throw new ArgumentNullException("apiName");

            return _jsonHttpClient.HttpGetRestApiAsync<T>(apiName, token);
        }

        public Task<T> ExecuteRestApiAsync<T>(string apiName, object inputObject, CancellationToken token)
        {
            if (string.IsNullOrEmpty(apiName)) throw new ArgumentNullException("apiName");
            if (inputObject == null) throw new ArgumentNullException("inputObject");

            return _jsonHttpClient.HttpPostRestApiAsync<T>(apiName, inputObject, token);
        }

		public async Task<T> QueryByIdAsync<T>(string objectName, string recordId, CancellationToken token)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(recordId)) throw new ArgumentNullException("recordId");

		    var fields = string.Join(", ", typeof(T).GetRuntimeProperties()
		        .Select(p => {
		            var customAttribute = p.GetCustomAttribute<DataMemberAttribute>();
		            return (customAttribute == null || customAttribute.Name == null) ? p.Name : customAttribute.Name;
		        }));

            var query = string.Format("SELECT {0} FROM {1} WHERE Id = '{2}'", fields, objectName, recordId);
            var results = await QueryAsync<T>(query, token).ConfigureAwait(false);

            return results.Records.FirstOrDefault();
        }

        public Task<SuccessResponse> CreateAsync(string objectName, object record, CancellationToken token)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (record == null) throw new ArgumentNullException("record");

            return _jsonHttpClient.HttpPostAsync<SuccessResponse>(record, string.Format("sobjects/{0}", objectName), token);
        }

        public Task<SuccessResponse> UpdateAsync(string objectName, string recordId, object record, CancellationToken token)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(recordId)) throw new ArgumentNullException("recordId");
            if (record == null) throw new ArgumentNullException("record");

            return _jsonHttpClient.HttpPatchAsync(record, string.Format("sobjects/{0}/{1}", objectName, recordId), token);
        }

        public Task<SuccessResponse> UpsertExternalAsync(string objectName, string externalFieldName, string externalId, object record, CancellationToken token)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(externalFieldName)) throw new ArgumentNullException("externalFieldName");
            if (string.IsNullOrEmpty(externalId)) throw new ArgumentNullException("externalId");
            if (record == null) throw new ArgumentNullException("record");

            return _jsonHttpClient.HttpPatchAsync(record, string.Format("sobjects/{0}/{1}/{2}", objectName, externalFieldName, externalId), token);
        }

        public Task<bool> DeleteAsync(string objectName, string recordId, CancellationToken token)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(recordId)) throw new ArgumentNullException("recordId");

            return _jsonHttpClient.HttpDeleteAsync(string.Format("sobjects/{0}/{1}", objectName, recordId), token);
        }

        public Task<bool> DeleteExternalAsync(string objectName, string externalFieldName, string externalId, CancellationToken token)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(externalFieldName)) throw new ArgumentNullException("externalFieldName");
            if (string.IsNullOrEmpty(externalId)) throw new ArgumentNullException("externalId");

            return _jsonHttpClient.HttpDeleteAsync(string.Format("sobjects/{0}/{1}/{2}", objectName, externalFieldName, externalId), token);
        }
        
        public Task<DescribeGlobalResult<T>> GetObjectsAsync<T>(CancellationToken token)
        {
            return _jsonHttpClient.HttpGetAsync<DescribeGlobalResult<T>>("sobjects", token);
        }

        public Task<T> BasicInformationAsync<T>(string objectName, CancellationToken token)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");

            return _jsonHttpClient.HttpGetAsync<T>(string.Format("sobjects/{0}", objectName), token);
        }

        public Task<T> DescribeAsync<T>(string objectName, CancellationToken token)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");

            return _jsonHttpClient.HttpGetAsync<T>(string.Format("sobjects/{0}/describe/", objectName), token);
        }

        public Task<T> GetDeleted<T>(string objectName, DateTime startDateTime, DateTime endDateTime, CancellationToken token)
        {
            var sdt = Uri.EscapeDataString(startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss+00:00", System.Globalization.CultureInfo.InvariantCulture));
            var edt = Uri.EscapeDataString(endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss+00:00", System.Globalization.CultureInfo.InvariantCulture));

            return _jsonHttpClient.HttpGetAsync<T>(string.Format("sobjects/{0}/deleted/?start={1}&end={2}", objectName, sdt, edt), token);
        }

        public Task<T> GetUpdated<T>(string objectName, DateTime startDateTime, DateTime endDateTime, CancellationToken token)
        {
            var sdt = Uri.EscapeDataString(startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss+00:00", System.Globalization.CultureInfo.InvariantCulture));
            var edt = Uri.EscapeDataString(endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss+00:00", System.Globalization.CultureInfo.InvariantCulture));

            return _jsonHttpClient.HttpGetAsync<T>(string.Format("sobjects/{0}/updated/?start={1}&end={2}", objectName, sdt, edt), token);
        }

        public Task<T> DescribeLayoutAsync<T>(string objectName, CancellationToken token)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            return _jsonHttpClient.HttpGetAsync<T>(string.Format("sobjects/{0}/describe/layouts/", objectName), token);
        }

        public Task<T> DescribeLayoutAsync<T>(string objectName, string recordTypeId, CancellationToken token)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(recordTypeId)) throw new ArgumentNullException("recordTypeId");

            return _jsonHttpClient.HttpGetAsync<T>(string.Format("sobjects/{0}/describe/layouts/{1}", objectName, recordTypeId), token);
        }

        public Task<T> RecentAsync<T>(int limit = 200, CancellationToken token = default(CancellationToken))
        {
            return _jsonHttpClient.HttpGetAsync<T>(string.Format("recent/?limit={0}", limit), token);
        }

        public Task<List<T>> SearchAsync<T>(string query, CancellationToken token)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException("query");
            if (!query.Contains("FIND")) throw new ArgumentException("query does not contain FIND");
            if (!query.Contains("{") || !query.Contains("}")) throw new ArgumentException("search term must be wrapped in braces");

            return _jsonHttpClient.HttpGetAsync<List<T>>(string.Format("search?q={0}", Uri.EscapeDataString(query)), token);
        }

        public Task<T> UserInfo<T>(string url, CancellationToken token)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) throw new FormatException("url");

            return _jsonHttpClient.HttpGetAsync<T>(new Uri(url), token);
        }

        // BULK METHODS

        public async Task<List<BatchInfoResult>> RunJobAsync<T>(string objectName, BulkConstants.OperationType operationType,
            IEnumerable<ISObjectList<T>> recordsLists, CancellationToken token)
        {
            if (recordsLists == null) throw new ArgumentNullException("recordsLists");

            var jobInfoResult = await CreateJobAsync(objectName, operationType, token).ConfigureAwait(false);
            var batchResults = new List<BatchInfoResult>();
            foreach (var recordList in recordsLists)
            {
	            var batchInfoResult = await CreateJobBatchAsync(jobInfoResult, recordList, token).ConfigureAwait(false);
	            batchResults.Add(batchInfoResult);
            }
            await CloseJobAsync(jobInfoResult, token).ConfigureAwait(false);
            return batchResults;
        }

        public async Task<List<BatchResultList>> RunJobAndPollAsync<T>(string objectName, BulkConstants.OperationType operationType,
            IEnumerable<ISObjectList<T>> recordsLists, CancellationToken token)
        {
            const float pollingStart = 1000;
            const float pollingIncrease = 2.0f;

            var batchInfoResults = await RunJobAsync(objectName, operationType, recordsLists, token).ConfigureAwait(false);

            var currentPoll = pollingStart;
            var finishedBatchInfoResults = new List<BatchInfoResult>();
            while (batchInfoResults.Count > 0)
            {
                var removeList = new List<BatchInfoResult>();
                foreach (var batchInfoResult in batchInfoResults)
                {
                    var batchInfoResultNew = await PollBatchAsync(batchInfoResult, token).ConfigureAwait(false);
                    if (batchInfoResultNew.State.Equals(BulkConstants.BatchState.Completed.Value()) ||
                        batchInfoResultNew.State.Equals(BulkConstants.BatchState.Failed.Value()) ||
                        batchInfoResultNew.State.Equals(BulkConstants.BatchState.NotProcessed.Value()))
                    {
                        finishedBatchInfoResults.Add(batchInfoResultNew);
                        removeList.Add(batchInfoResult);
                    }
                }
                foreach (var removeItem in removeList)
                {
                    batchInfoResults.Remove(removeItem);
                }

                await Task.Delay((int)currentPoll, token).ConfigureAwait(false);
                currentPoll *= pollingIncrease;
            }


            var batchResults = new List<BatchResultList>();
            foreach (var batchInfoResultComplete in finishedBatchInfoResults)
            {
	            var batchResultList = await GetBatchResultAsync(batchInfoResultComplete, token).ConfigureAwait(false);
	            batchResults.Add(batchResultList);
            }
            return batchResults;
        }

        public Task<JobInfoResult> CreateJobAsync(string objectName, BulkConstants.OperationType operationType, CancellationToken token)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");

            var jobInfo = new JobInfo
            {
                ContentType = "XML",
                Object = objectName,
                Operation = operationType.Value()
            };

            return _xmlHttpClient.HttpPostAsync<JobInfoResult>(jobInfo, "/services/async/{0}/job", token);
        }

        public Task<BatchInfoResult> CreateJobBatchAsync<T>(JobInfoResult jobInfo, ISObjectList<T> recordsList, CancellationToken token)
        {
            if (jobInfo == null) throw new ArgumentNullException("jobInfo");
            return CreateJobBatchAsync(jobInfo.Id, recordsList, token);
        }

        public Task<BatchInfoResult> CreateJobBatchAsync<T>(string jobId, ISObjectList<T> recordsObject, CancellationToken token)
        {
            if (string.IsNullOrEmpty(jobId)) throw new ArgumentNullException("jobId");
            if (recordsObject == null) throw new ArgumentNullException("recordsObject");

            return _xmlHttpClient.HttpPostAsync<BatchInfoResult>(recordsObject, string.Format("/services/async/{{0}}/job/{0}/batch", jobId), token);
        }

        public Task<JobInfoResult> CloseJobAsync(JobInfoResult jobInfo, CancellationToken token)
        {
            if (jobInfo == null) throw new ArgumentNullException("jobInfo");
            return CloseJobAsync(jobInfo.Id, token);
        }

        public Task<JobInfoResult> CloseJobAsync(string jobId, CancellationToken token)
        {
            if (string.IsNullOrEmpty(jobId)) throw new ArgumentNullException("jobId");

            var state = new JobInfoState { State = "Closed" };
            return _xmlHttpClient.HttpPostAsync<JobInfoResult>(state, string.Format("/services/async/{{0}}/job/{0}", jobId), token);
        }

        public Task<JobInfoResult> PollJobAsync(JobInfoResult jobInfo, CancellationToken token)
        {
            if (jobInfo == null) throw new ArgumentNullException("jobInfo");
            return PollJobAsync(jobInfo.Id, token);
        }

        public Task<JobInfoResult> PollJobAsync(string jobId, CancellationToken token)
        {
            if (string.IsNullOrEmpty(jobId)) throw new ArgumentNullException("jobId");

	        return _xmlHttpClient.HttpGetAsync<JobInfoResult>(string.Format("/services/async/{{0}}/job/{0}", jobId), token);
        }

        public Task<BatchInfoResult> PollBatchAsync(BatchInfoResult batchInfo, CancellationToken token)
        {
            if (batchInfo == null) throw new ArgumentNullException("batchInfo");
            return PollBatchAsync(batchInfo.Id, batchInfo.JobId, token);
        }

        public Task<BatchInfoResult> PollBatchAsync(string batchId, string jobId, CancellationToken token)
        {
            if (string.IsNullOrEmpty(batchId)) throw new ArgumentNullException("batchId");
            if (string.IsNullOrEmpty(jobId)) throw new ArgumentNullException("jobId");

            return _xmlHttpClient.HttpGetAsync<BatchInfoResult>(string.Format("/services/async/{{0}}/job/{0}/batch/{1}", jobId, batchId), token);
        }

        public Task<BatchResultList> GetBatchResultAsync(BatchInfoResult batchInfo, CancellationToken token)
        {
            if (batchInfo == null) throw new ArgumentNullException("batchInfo");
            return GetBatchResultAsync(batchInfo.Id, batchInfo.JobId, token);
        }

        public Task<BatchResultList> GetBatchResultAsync(string batchId, string jobId, CancellationToken token)
        {
            if (string.IsNullOrEmpty(batchId)) throw new ArgumentNullException("batchId");
            if (string.IsNullOrEmpty(jobId)) throw new ArgumentNullException("jobId");

            return _xmlHttpClient.HttpGetAsync<BatchResultList>(string.Format("/services/async/{{0}}/job/{0}/batch/{1}/result", jobId, batchId), token);
        }

        public void Dispose()
        {
            _jsonHttpClient.Dispose();
            _xmlHttpClient.Dispose();
        }
    }
}
