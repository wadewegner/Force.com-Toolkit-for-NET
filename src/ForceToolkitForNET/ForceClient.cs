using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Salesforce.Common;
using Salesforce.Common.Models;

namespace Salesforce.Force
{
    public class ForceClient : IForceClient
    {
        private static ServiceHttpClient _serviceHttpClient;
        private static string _userAgent = "forcedotcom-libraries-dotnet";
        
        public ForceClient(string instanceUrl, string accessToken, string apiVersion) 
            : this (instanceUrl, accessToken, apiVersion, new HttpClient())
        {
        }

        public ForceClient(string instanceUrl, string accessToken, string apiVersion, HttpClient httpClient)
        {
            _serviceHttpClient = new ServiceHttpClient(instanceUrl, apiVersion, accessToken, _userAgent, httpClient);
        }
      
        public async Task<IList<T>> QueryAsync<T>(string query)
        {
            var response = await _serviceHttpClient.HttpGetAsync<IList<T>>(string.Format("query?q={0}", query), "records");
            return response;
        }

        public async Task<T> QueryByIdAsync<T>(string objectName, string recordId)
        {
            var fields = string.Join(", ", typeof(T).GetProperties().Select(p => p.Name));
            var query = string.Format("SELECT {0} FROM {1} WHERE Id = '{2}'", fields, objectName, recordId);
            var results = await QueryAsync<T>(query);

            return results.FirstOrDefault();
        }

        public async Task<string> CreateAsync(string objectName, object record)
        {
            var response = await _serviceHttpClient.HttpPostAsync<SuccessResponse>(record, string.Format("sobjects/{0}", objectName));
            return response.id;
        }

        public async Task<bool> UpdateAsync(string objectName, string recordId, object record)
        {
            var response = await _serviceHttpClient.HttpPatchAsync(record, string.Format("sobjects/{0}/{1}", objectName, recordId));
            return response;
        }

        public async Task<bool> DeleteAsync(string objectName, string recordId)
        {
            var response = await _serviceHttpClient.HttpDeleteAsync(string.Format("sobjects/{0}/{1}", objectName, recordId));
            return response;
        }

        public async Task<IList<T>> GetObjectsAsync<T>()
        {
            var response = await _serviceHttpClient.HttpGetAsync<IList<T>>("sobjects", "sobjects");
            return response;
        }

        public async Task<T> DescribeAsync<T>(string objectName)
        {
            var response = await _serviceHttpClient.HttpGetAsync<T>(string.Format("sobjects/{0}", objectName), "objectDescribe");
            return response;
        }

        public async Task<T> RecentAsync<T>(int limit = 200)
        {
            var response = await _serviceHttpClient.HttpGetAsync<T>(string.Format("recent/?limit={0}", limit));
            return response;
        }
    }
}
