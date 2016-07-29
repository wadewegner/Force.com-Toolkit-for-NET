using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Salesforce.Common.Models.Json;
using Salesforce.Common.Models.Xml;

namespace Salesforce.Force
{
    public interface IForceClient: IDisposable
    {

        // STANDARD
        Task<QueryResult<T>> QueryAsync<T>(string query);
        Task<QueryResult<T>> QueryContinuationAsync<T>(string nextRecordsUrl);
        Task<QueryResult<T>> QueryAllAsync<T>(string query);
        Task<T> QueryByIdAsync<T>(string objectName, string recordId);
        Task<T> ExecuteRestApiAsync<T>(string apiName);
        Task<T> ExecuteRestApiAsync<T>(string apiName, object inputObject);
        Task<SuccessResponse> CreateAsync(string objectName, object record);
        Task<SuccessResponse> UpdateAsync(string objectName, string recordId, object record);
        Task<SuccessResponse> UpsertExternalAsync(string objectName, string externalFieldName, string externalId, object record);
        Task<bool> DeleteAsync(string objectName, string recordId);
        Task<bool> DeleteExternalAsync(string objectName, string externalFieldName, string externalId);
        Task<DescribeGlobalResult<T>> GetObjectsAsync<T>();
        Task<T> BasicInformationAsync<T>(string objectName);
        Task<T> DescribeAsync<T>(string objectName);
        Task<T> GetDeleted<T>(string objectName, DateTime startDateTime, DateTime endDateTime);
        Task<T> GetUpdated<T>(string objectName, DateTime startDateTime, DateTime endDateTime);
        Task<T> DescribeLayoutAsync<T>(string objectName);
        Task<T> DescribeLayoutAsync<T>(string objectName, string recordTypeId);
        Task<T> RecentAsync<T>(int limit = 200);
        Task<List<T>> SearchAsync<T>(string query);
        Task<T> UserInfo<T>(string url);

        // BULK
        Task<List<BatchInfoResult>> RunJobAsync<T>(string objectName, BulkConstants.OperationType operationType, IEnumerable<ISObjectList<T>> recordsLists);
        Task<List<BatchResultList>> RunJobAndPollAsync<T>(string objectName, BulkConstants.OperationType operationType, IEnumerable<ISObjectList<T>> recordsLists);
        Task<JobInfoResult> CreateJobAsync(string objectName, BulkConstants.OperationType operationType);
        Task<BatchInfoResult> CreateJobBatchAsync<T>(JobInfoResult jobInfo, ISObjectList<T> recordsObject);
        Task<BatchInfoResult> CreateJobBatchAsync<T>(string jobId, ISObjectList<T> recordsObject);
        Task<JobInfoResult> CloseJobAsync(JobInfoResult jobInfo);
        Task<JobInfoResult> CloseJobAsync(string jobId);
        Task<JobInfoResult> PollJobAsync(JobInfoResult jobInfo);
        Task<JobInfoResult> PollJobAsync(string jobId);
        Task<BatchInfoResult> PollBatchAsync(BatchInfoResult batchInfo);
        Task<BatchInfoResult> PollBatchAsync(string batchId, string jobId);
        Task<BatchResultList> GetBatchResultAsync(BatchInfoResult batchInfo);
        Task<BatchResultList> GetBatchResultAsync(string batchId, string jobId);
    }
}
