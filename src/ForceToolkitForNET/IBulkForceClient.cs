using System.Collections.Generic;
using System.Threading.Tasks;
using Salesforce.Common.Models.Xml;

namespace Salesforce.Force
{
    public interface IBulkForceClient
    {
        Task<JobInfoResult> CreateJobAsync(string objectName, BulkOperationType operationType);
        Task<BatchInfoResult> CreateJobBatchAsync<T>(JobInfoResult jobInfo, List<T> recordsList);
        Task<BatchInfoResult> CreateJobBatchAsync<T>(string jobId, List<T> recordList);
    }
}
