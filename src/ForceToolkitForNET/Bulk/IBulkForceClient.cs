using System.Threading.Tasks;
using Salesforce.Force.Bulk.Models;

namespace Salesforce.Force.Bulk
{
    public interface IBulkForceClient
    {
        Task<JobInfoResult> CreateJobAsync(string objectName, Bulk.OperationType operationType);
        Task<BatchInfoResult> CreateJobBatchAsync(JobInfoResult jobInfo, ISObjectList recordsObject);
        Task<BatchInfoResult> CreateJobBatchAsync(string jobId, ISObjectList recordsObject);
        Task<JobInfoResult> CloseJobAsync(JobInfoResult jobInfo);
        Task<JobInfoResult> CloseJobAsync(string jobId);
        Task<JobInfoResult> PollJobAsync(JobInfoResult jobInfo);
        Task<JobInfoResult> PollJobAsync(string jobId);
        Task<BatchInfoResult> PollBatchAsync(BatchInfoResult batchInfo);
        Task<BatchInfoResult> PollBatchAsync(string batchId, string jobId);
        Task<BatchResultList> GetBatchResult(BatchInfoResult batchInfo);
        Task<BatchResultList> GetBatchResult(string batchId, string jobId);
    }
}
