using System.Collections.Generic;
using System.Threading.Tasks;

namespace Salesforce.Force
{
    interface IForceClient
    {
        Task<IList<T>> QueryAsync<T>(string query);
        Task<string> CreateAsync(string objectName, object record);
        Task<bool> UpdateAsync(string objectName, string recordId, object record);
        Task<bool> DeleteAsync(string objectName, string recordId);
        Task<T> QueryByIdAsync<T>(string objectName, string recordId);
        Task<IList<T>> GetObjectsAsync<T>();
        Task<T> DescribeAsync<T>(string objectName);
        Task<T> RecentAsync<T>(int limit);
    }
}
