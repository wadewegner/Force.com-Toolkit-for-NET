using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Reflection;
using Salesforce.Common;
using Salesforce.Common.Models;

namespace Salesforce.Force
{
    public class ForceClient : IForceClient, IDisposable
    {
        private readonly ServiceHttpClient _serviceHttpClient;

        public ForceClient(string instanceUrl, string accessToken, string apiVersion, HttpClient httpClient = null)
        {
            if (string.IsNullOrEmpty(instanceUrl)) throw new ArgumentNullException("instanceUrl");
            if (string.IsNullOrEmpty(accessToken)) throw new ArgumentNullException("accessToken");
            if (string.IsNullOrEmpty(apiVersion)) throw new ArgumentNullException("apiVersion");

            _serviceHttpClient = new ServiceHttpClient(instanceUrl, apiVersion, accessToken, httpClient);
        }

        public Task<QueryResult<T>> QueryAsync<T>(string query)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException("query");

            return _serviceHttpClient.HttpGetAsync<QueryResult<T>>(string.Format("query?q={0}", Uri.EscapeDataString(query)));
        }

        public Task<QueryResult<T>> QueryContinuationAsync<T>(string nextRecordsUrl)
        {
            if (string.IsNullOrEmpty(nextRecordsUrl)) throw new ArgumentNullException("nextRecordsUrl");

            return _serviceHttpClient.HttpGetAsync<QueryResult<T>>(nextRecordsUrl);
        }

        public Task<QueryResult<T>> QueryAllAsync<T>(string query)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException("query");

            return _serviceHttpClient.HttpGetAsync<QueryResult<T>>(string.Format("queryAll/?q={0}", Uri.EscapeDataString(query)));
        }
        
        public async Task<T> ExecuteRestApiAsync<T>(string apiName)
        {
            if (string.IsNullOrEmpty(apiName)) throw new ArgumentNullException("apiName");

            var response = await _serviceHttpClient.HttpGetRestApiAsync<T>(apiName);
            return response;
        }

        public async Task<T> ExecuteRestApiAsync<T>(string apiName, object inputObject)
        {
            if (string.IsNullOrEmpty(apiName)) throw new ArgumentNullException("apiName");
            if (inputObject == null) throw new ArgumentNullException("inputObject");

            var response = await _serviceHttpClient.HttpPostRestApiAsync<T>(apiName, inputObject);
            return response;
        }
        
		public async Task<T> QueryByIdAsync<T>(string objectName, string recordId)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(recordId)) throw new ArgumentNullException("recordId");

		    var fields = string.Join(", ", typeof(T).GetRuntimeProperties()
		        .Select(p => { 
		            var customAttribute = p.GetCustomAttribute<DataMemberAttribute>();
		            return (customAttribute == null || customAttribute.Name == null) ? p.Name : customAttribute.Name;
		        }));

            var query = string.Format("SELECT {0} FROM {1} WHERE Id = '{2}'", fields, objectName, recordId);
            var results = await QueryAsync<T>(query).ConfigureAwait(false);

            return results.Records.FirstOrDefault();
        }

        public async Task<SuccessResponse> CreateAsync(string objectName, object record)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (record == null) throw new ArgumentNullException("record");

            return await _serviceHttpClient.HttpPostAsync<SuccessResponse>(record, string.Format("sobjects/{0}", objectName)).ConfigureAwait(false);
        }

        public Task<SuccessResponse> UpdateAsync(string objectName, string recordId, object record)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(recordId)) throw new ArgumentNullException("recordId");
            if (record == null) throw new ArgumentNullException("record");

            return _serviceHttpClient.HttpPatchAsync(record, string.Format("sobjects/{0}/{1}", objectName, recordId));
        }

        public Task<SuccessResponse> UpsertExternalAsync(string objectName, string externalFieldName, string externalId, object record)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(externalFieldName)) throw new ArgumentNullException("externalFieldName");
            if (string.IsNullOrEmpty(externalId)) throw new ArgumentNullException("externalId");
            if (record == null) throw new ArgumentNullException("record");

            return _serviceHttpClient.HttpPatchAsync(record, string.Format("sobjects/{0}/{1}/{2}", objectName, externalFieldName, externalId));
        }

        public Task<bool> DeleteAsync(string objectName, string recordId)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(recordId)) throw new ArgumentNullException("recordId");

            return _serviceHttpClient.HttpDeleteAsync(string.Format("sobjects/{0}/{1}", objectName, recordId));
        }

        public Task<bool> DeleteExternalAsync(string objectName, string externalFieldName, string externalId)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(externalFieldName)) throw new ArgumentNullException("externalFieldName");
            if (string.IsNullOrEmpty(externalId)) throw new ArgumentNullException("externalId");

            return _serviceHttpClient.HttpDeleteAsync(string.Format("sobjects/{0}/{1}/{2}", objectName, externalFieldName, externalId));
        }
        
        public Task<DescribeGlobalResult<T>> GetObjectsAsync<T>()
        {
            return _serviceHttpClient.HttpGetAsync<DescribeGlobalResult<T>>("sobjects");
        }

        public Task<T> BasicInformationAsync<T>(string objectName)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");

            return _serviceHttpClient.HttpGetAsync<T>(string.Format("sobjects/{0}", objectName));
        }

        public Task<T> DescribeAsync<T>(string objectName)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");

            return _serviceHttpClient.HttpGetAsync<T>(string.Format("sobjects/{0}/describe/", objectName));
        }

        public Task<T> GetDeleted<T>(string objectName, DateTime startDateTime, DateTime endDateTime)
        {
            var sdt = Uri.EscapeDataString(startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss+00:00", System.Globalization.CultureInfo.InvariantCulture));
            var edt = Uri.EscapeDataString(endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss+00:00", System.Globalization.CultureInfo.InvariantCulture));

            return _serviceHttpClient.HttpGetAsync<T>(string.Format("sobjects/{0}/deleted/?start={1}&end={2}", objectName, sdt, edt));
        }

        public Task<T> GetUpdated<T>(string objectName, DateTime startDateTime, DateTime endDateTime)
        {
            var sdt = Uri.EscapeDataString(startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss+00:00", System.Globalization.CultureInfo.InvariantCulture));
            var edt = Uri.EscapeDataString(endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss+00:00", System.Globalization.CultureInfo.InvariantCulture));

            return _serviceHttpClient.HttpGetAsync<T>(string.Format("sobjects/{0}/updated/?start={1}&end={2}", objectName, sdt, edt));
        }
        
        public Task<T> DescribeLayoutAsync<T>(string objectName)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            
            return _serviceHttpClient.HttpGetAsync<T>(string.Format("sobjects/{0}/describe/layouts/", objectName));
        }
        
        public Task<T> DescribeLayoutAsync<T>(string objectName, string recordTypeId)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(recordTypeId)) throw new ArgumentNullException("recordTypeId");
            
            return _serviceHttpClient.HttpGetAsync<T>(string.Format("sobjects/{0}/describe/layouts/{1}", objectName, recordTypeId));
        }

        public Task<T> RecentAsync<T>(int limit = 200)
        {
            return _serviceHttpClient.HttpGetAsync<T>(string.Format("recent/?limit={0}", limit));
        }

        public Task<List<T>> SearchAsync<T>(string query)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException("query");
            if (!query.Contains("FIND")) throw new ArgumentException("query does not contain FIND");
            if (!query.Contains("{") || !query.Contains("}")) throw new ArgumentException("search term must be wrapped in braces");

            return _serviceHttpClient.HttpGetAsync<List<T>>(string.Format("search?q={0}", Uri.EscapeDataString(query)));
        }

        public async Task<T> UserInfo<T>(string url)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) throw new FormatException("url");

            var response = await _serviceHttpClient.HttpGetAsync<T>(new Uri(url));
            return response;
        }

        public void Dispose()
        {
            _serviceHttpClient.Dispose();
        }
    }
}
