using Salesforce.Common.Models.Xml;
using Salesforce.Force;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Salesforce.Force
{
    /// <summary>
    /// Extends ForceClient for creating bulk upsert jobs using new UpsertJobInfo.
    /// 
    /// For a complete list of possible JobInfo fields, see https://developer.salesforce.com/docs/atlas.en-us.api_asynch.meta/api_asynch/asynch_api_reference_jobinfo.htm
    /// </summary>
    /// <remarks>
    /// This class requires the following properties in ForceClient to be minimally exposed as protected:
    /// - _xmlHttpClient
    /// - _jsonHttpClient
    /// </remarks>
    public class BulkForceClient : ForceClient, IDisposable, IBulkForceClient
    {
        public BulkForceClient(string instanceUrl, string accessToken, string apiVersion)
            : this(instanceUrl, accessToken, apiVersion, new HttpClient(), new HttpClient())
        {
        }

        public BulkForceClient(string instanceUrl, string accessToken, string apiVersion, HttpClient httpClientForJson, HttpClient httpClientForXml)
            : base(instanceUrl, accessToken, apiVersion, httpClientForJson, httpClientForXml)
        {
        }

        public async Task<JobInfoResult> CreateUpsertJobAsync(string objectName, string externalField, BulkConstants.OperationType operationType)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");

            var jobInfo = new UpsertJobInfo
            {
                ContentType = "XML",
                Object = objectName,
                ExternalField = externalField,
                Operation = operationType.Value()
            };

            return await _xmlHttpClient.HttpPostAsync<JobInfoResult>(jobInfo, "/services/async/{0}/job");
        }

        public async Task<List<BatchInfoResult>> RunUpsertJobAsync<T>(string objectName, string externalFieldName, BulkConstants.OperationType operationType,
            IEnumerable<ISObjectList<T>> recordsLists)
        {
            if (recordsLists == null) throw new ArgumentNullException("recordsLists");

            var jobInfoResult = await CreateUpsertJobAsync(objectName, externalFieldName, operationType);
            var batchResults = new List<BatchInfoResult>();
            foreach (var recordList in recordsLists)
            {
                batchResults.Add(await CreateJobBatchAsync(jobInfoResult, recordList));
            }
            await CloseJobAsync(jobInfoResult);
            return batchResults;
        }

        public async Task<List<BatchResultList>> RunUpsertJobAndPollAsync<T>(string objectName, string externalFieldName, BulkConstants.OperationType operationType,
            IEnumerable<ISObjectList<T>> recordsLists)
        {
            const float pollingStart = 1000;
            const float pollingIncrease = 2.0f;

            var batchInfoResults = await RunUpsertJobAsync(objectName, externalFieldName, operationType, recordsLists);

            var currentPoll = pollingStart;
            var finishedBatchInfoResults = new List<BatchInfoResult>();
            while (batchInfoResults.Count > 0)
            {
                var removeList = new List<BatchInfoResult>();
                foreach (var batchInfoResult in batchInfoResults)
                {
                    var batchInfoResultNew = await PollBatchAsync(batchInfoResult);
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

                await Task.Delay((int)currentPoll);
                currentPoll *= pollingIncrease;
            }


            var batchResults = new List<BatchResultList>();
            foreach (var batchInfoResultComplete in finishedBatchInfoResults)
            {
                batchResults.Add(await GetBatchResultAsync(batchInfoResultComplete));
            }
            return batchResults;
        }
    }
}
