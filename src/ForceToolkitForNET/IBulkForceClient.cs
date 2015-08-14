using System.Collections.Generic;
using System.Threading.Tasks;
using Salesforce.Common.Models.Xml;

namespace Salesforce.Force
{
    public interface IBulkForceClient
    {
        Task<JobInfoResult> CreateJobAsync(string objectName, Bulk.OperationType operationType, Bulk.ConcurrencyMode concurrencyMode);
        Task<BatchInfoResult> CreateJobBatchAsync<T>(JobInfoResult jobInfo, List<T> recordsList);
        Task<BatchInfoResult> CreateJobBatchAsync<T>(string jobId, List<T> recordList);
        Task<BatchInfoResult> CreateJobBatchAsync(JobInfoResult jobInfo, string csvData);
        Task<BatchInfoResult> CreateJobBatchAsync(string jobId, string csvData);
        Task<JobInfoResult> CloseJobAsync(JobInfoResult jobInfo);
        Task<JobInfoResult> CloseJobAsync(string jobId);
        Task<JobInfoResult> PollJobAsync(JobInfoResult jobInfo);
        Task<JobInfoResult> PollJobAsync(string jobId);
        Task<BatchInfoResult> PollBatchAsync(BatchInfoResult batchInfo);
        Task<BatchInfoResult> PollBatchAsync(string batchId, string jobId);
    }
}
