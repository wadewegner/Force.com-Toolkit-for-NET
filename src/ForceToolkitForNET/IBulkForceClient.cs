using System.Collections.Generic;
using System.Threading.Tasks;
using Salesforce.Common.Models.Xml;
using Salesforce.Force;

namespace Salesforce.Force
{
    public interface IBulkForceClient
    {
        Task<JobInfoResult> CreateUpsertJobAsync(string objectName, string externalField, BulkConstants.OperationType operationType);
        Task<List<BatchResultList>> RunUpsertJobAndPollAsync<T>(string objectName, string externalFieldName, BulkConstants.OperationType operationType, IEnumerable<ISObjectList<T>> recordsLists);
        Task<List<BatchInfoResult>> RunUpsertJobAsync<T>(string objectName, string externalFieldName, BulkConstants.OperationType operationType, IEnumerable<ISObjectList<T>> recordsLists);
    }
}
