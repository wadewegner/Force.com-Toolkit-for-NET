//TODO: add license header

using System.Threading.Tasks;
using Salesforce.Common.Models;

namespace Salesforce.Force
{
    public interface IForceClient
    {
        Task<QueryResult<T>> QueryAsync<T>(string query);
        Task<string> CreateAsync(string objectName, object record);
        Task<bool> UpdateAsync(string objectName, string recordId, object record);
        Task<bool> UpsertExternalAsync(string objectName, string externalId, string recordId, object record);
        Task<bool> DeleteAsync(string objectName, string recordId);
        Task<T> QueryByIdAsync<T>(string objectName, string recordId);
        Task<DescribeGlobalResult<T>> GetObjectsAsync<T>();
        Task<T> DescribeAsync<T>(string objectName);
        Task<T> RecentAsync<T>(int limit);
    }
}
