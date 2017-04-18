using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Salesforce.Common.Models.Json;
using Salesforce.Common.Models.Xml;

namespace Salesforce.Force
{
    public interface IForceClient: IDisposable
    {

        // STANDARD
        Task<QueryResult<T>> QueryAsync<T>(string query, CancellationToken token = default(CancellationToken));
        Task<QueryResult<T>> QueryContinuationAsync<T>(string nextRecordsUrl, CancellationToken token = default(CancellationToken));
        Task<QueryResult<T>> QueryAllAsync<T>(string query, CancellationToken token = default(CancellationToken));
        Task<T> QueryByIdAsync<T>(string objectName, string recordId, CancellationToken token = default(CancellationToken));
        Task<T> ExecuteRestApiAsync<T>(string apiName, CancellationToken token = default(CancellationToken));
        Task<T> ExecuteRestApiAsync<T>(string apiName, object inputObject, CancellationToken token = default(CancellationToken));
        Task<SuccessResponse> CreateAsync(string objectName, object record, CancellationToken token = default(CancellationToken));
        Task<SuccessResponse> UpdateAsync(string objectName, string recordId, object record, CancellationToken token = default(CancellationToken));
        Task<SuccessResponse> UpsertExternalAsync(string objectName, string externalFieldName, string externalId, object record, CancellationToken token = default(CancellationToken));
        Task<bool> DeleteAsync(string objectName, string recordId, CancellationToken token = default(CancellationToken));
        Task<bool> DeleteExternalAsync(string objectName, string externalFieldName, string externalId, CancellationToken token = default(CancellationToken));
        Task<DescribeGlobalResult<T>> GetObjectsAsync<T>(CancellationToken token = default(CancellationToken));
        Task<T> BasicInformationAsync<T>(string objectName, CancellationToken token = default(CancellationToken));
        Task<T> DescribeAsync<T>(string objectName, CancellationToken token = default(CancellationToken));
        Task<T> GetDeleted<T>(string objectName, DateTime startDateTime, DateTime endDateTime, CancellationToken token = default(CancellationToken));
        Task<T> GetUpdated<T>(string objectName, DateTime startDateTime, DateTime endDateTime, CancellationToken token = default(CancellationToken));
        Task<T> DescribeLayoutAsync<T>(string objectName, CancellationToken token = default(CancellationToken));
        Task<T> DescribeLayoutAsync<T>(string objectName, string recordTypeId, CancellationToken token = default(CancellationToken));
        Task<T> RecentAsync<T>(int limit = 200, CancellationToken token = default(CancellationToken));
        Task<List<T>> SearchAsync<T>(string query, CancellationToken token = default(CancellationToken));
        Task<T> UserInfo<T>(string url, CancellationToken token = default(CancellationToken));

        // BULK
        Task<List<BatchInfoResult>> RunJobAsync<T>(string objectName, BulkConstants.OperationType operationType, IEnumerable<ISObjectList<T>> recordsLists, CancellationToken token = default(CancellationToken));
        Task<List<BatchResultList>> RunJobAndPollAsync<T>(string objectName, BulkConstants.OperationType operationType, IEnumerable<ISObjectList<T>> recordsLists, CancellationToken token = default(CancellationToken));
        Task<JobInfoResult> CreateJobAsync(string objectName, BulkConstants.OperationType operationType, CancellationToken token = default(CancellationToken));
        Task<BatchInfoResult> CreateJobBatchAsync<T>(JobInfoResult jobInfo, ISObjectList<T> recordsObject, CancellationToken token = default(CancellationToken));
        Task<BatchInfoResult> CreateJobBatchAsync<T>(string jobId, ISObjectList<T> recordsObject, CancellationToken token = default(CancellationToken));
        Task<JobInfoResult> CloseJobAsync(JobInfoResult jobInfo, CancellationToken token = default(CancellationToken));
        Task<JobInfoResult> CloseJobAsync(string jobId, CancellationToken token = default(CancellationToken));
        Task<JobInfoResult> PollJobAsync(JobInfoResult jobInfo, CancellationToken token = default(CancellationToken));
        Task<JobInfoResult> PollJobAsync(string jobId, CancellationToken token = default(CancellationToken));
        Task<BatchInfoResult> PollBatchAsync(BatchInfoResult batchInfo, CancellationToken token = default(CancellationToken));
        Task<BatchInfoResult> PollBatchAsync(string batchId, string jobId, CancellationToken token = default(CancellationToken));
        Task<BatchResultList> GetBatchResultAsync(BatchInfoResult batchInfo, CancellationToken token = default(CancellationToken));
        Task<BatchResultList> GetBatchResultAsync(string batchId, string jobId, CancellationToken token = default(CancellationToken));
    }
}
