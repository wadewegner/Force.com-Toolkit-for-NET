using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForceSDKforNET
{
    interface IForceClient
    {
        string ApiVersion { get; set; }
        string InstanceUrl { get; set; }
        string AccessToken { get; set; }

        Task Authenticate(string clientId, string clientSecret, string username, string password);
        Task<IList<T>> Query<T>(string query);
        Task<string> Create(string objectName, object record);
        Task<bool> Update(string objectName, string recordId, object record);
        Task<bool> Delete(string objectName, string recordId);
    }
}
