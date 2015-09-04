using System.Collections.Generic;
using System.Threading.Tasks;
using Salesforce.Common.Models.Xml;

namespace Salesforce.Force.Bulk
{
    public interface IBulkForceClient
    {
        Task<List<BatchInfoResult>> RunJobAsync<T>(string objectName, Bulk.OperationType operationType, IEnumerable<ISObjectList<T>> recordsLists);
        Task<List<BatchResultList>> RunJobAndPollAsync<T>(string objectName, Bulk.OperationType operationType, IEnumerable<ISObjectList<T>> recordsLists);

        Task<JobInfoResult> CreateJobAsync(string objectName, Bulk.OperationType operationType);
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
