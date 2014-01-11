using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Salesforce.Common;
using Salesforce.Common.Models;

namespace Salesforce.Force
{
    public class ForceClient : IForceClient
    {
        public string InstanceUrl { get; set; }
        public string AccessToken { get; set; }
        public string ApiVersion { get; set; }

        private static ServiceHttpClient _httpClient;
        
        public ForceClient(string instanceUrl, string accessToken, string apiVersion)
        {
            this.InstanceUrl = instanceUrl;
            this.AccessToken = accessToken;
            this.ApiVersion = apiVersion;

            const string userAgent = "forcedotcom-toolkit-dotnet";

            _httpClient = new ServiceHttpClient(instanceUrl, apiVersion, accessToken, userAgent);
        }

        public ForceClient(string instanceUrl, string accessToken, string apiVersion, ServiceHttpClient httpClient)
        {
            this.InstanceUrl = instanceUrl;
            this.AccessToken = accessToken;
            this.ApiVersion = apiVersion;

            const string userAgent = "forcedotcom-toolkit-dotnet";

            _httpClient = httpClient;
        }
      
        public async Task<IList<T>> Query<T>(string query)
        {
            var response = await _httpClient.HttpGet<IList<T>>(string.Format("query?q={0}", query), "records");
            return response;
        }

        public async Task<T> QueryById<T>(string objectName, string recordId)
        {
            var fields = string.Join(", ", typeof(T).GetProperties().Select(p => p.Name));
            var query = string.Format("SELECT {0} FROM {1} WHERE Id = '{2}'", fields, objectName, recordId);
            var results = await Query<T>(query);

            return results.FirstOrDefault();
        }

        public async Task<string> Create(string objectName, object record)
        {
            var response = await _httpClient.HttpPost<SuccessResponse>(record, string.Format("sobjects/{0}", objectName));
            return response.id;
        }

        public async Task<bool> Update(string objectName, string recordId, object record)
        {
            var response = await _httpClient.HttpPatch(record, string.Format("sobjects/{0}/{1}", objectName, recordId));
            return response;
        }

        public async Task<bool> Delete(string objectName, string recordId)
        {
            var response = await _httpClient.HttpDelete(string.Format("sobjects/{0}/{1}", objectName, recordId));
            return response;
        }

        public async Task<IList<T>> GetObjects<T>()
        {
            var response = await _httpClient.HttpGet<IList<T>>("sobjects", "sobjects");
            return response;
        }

        public async Task<T> Describe<T>(string objectName)
        {
            var response = await _httpClient.HttpGet<T>(string.Format("sobjects/{0}", objectName), "objectDescribe");
            return response;
        }
    }
}
