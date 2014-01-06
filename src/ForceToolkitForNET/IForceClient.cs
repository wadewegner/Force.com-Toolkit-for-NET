using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForceToolkitForNET
{
    interface IForceClient
    {
        string InstanceUrl { get; set; }
        string AccessToken { get; set; }
        string ApiVersion { get; set; }
        Task<IList<T>> Query<T>(string query);
        Task<string> Create(string objectName, object record);
        Task<bool> Update(string objectName, string recordId, object record);
        Task<bool> Delete(string objectName, string recordId);
        Task<T> QueryById<T>(string objectName, string recordId);
        Task<IList<T>> GetObjects<T>();
        Task<T> Describe<T>(string objectName);
    }
}
