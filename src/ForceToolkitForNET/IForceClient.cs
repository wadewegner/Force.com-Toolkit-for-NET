//TODO: add license header

using System.Threading.Tasks;

namespace Salesforce.Force
{
    interface IForceClient
    {
        //Task<IList<T>> Query<T>(string query);
        Task<T> Query<T>(string query);
        Task<string> Create(string objectName, object record);
        Task<bool> Update(string objectName, string recordId, object record);
        Task<bool> Delete(string objectName, string recordId);
        Task<T> QueryById<T>(string objectName, string recordId);
        //Task<IList<T>> GetObjects<T>();
        Task<T> GetObjects<T>();
        Task<T> Describe<T>(string objectName);
        Task<T> Recent<T>(int limit);
    }
}
