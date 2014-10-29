using System.Threading.Tasks;
using Salesforce.Common.Models;

namespace Salesforce.Force
{
    public interface IForceClient
    {
        Task<QueryResult<T>> QueryAsync<T>(string query);
        Task<string> CreateAsync(string objectName, object record);
        Task<SuccessResponse> UpdateAsync(string objectName, string recordId, object record);
        Task<SuccessResponse> UpsertExternalAsync(string objectName, string externalId, string recordId, object record);
        Task<bool> DeleteAsync(string objectName, string recordId);
        Task<T> QueryByIdAsync<T>(string objectName, string recordId);
        Task<DescribeGlobalResult<T>> GetObjectsAsync<T>();
        Task<T> BasicInformationAsync<T>(string objectName);
        Task<T> DescribeAsync<T>(string objectName);
        Task<T> DescribeLayoutAsync<T>(string objectName);
        Task<T> DescribeLayoutAsync<T>(string objectName, string recordTypeId);
        Task<T> RecentAsync<T>(int limit);
    }
}
