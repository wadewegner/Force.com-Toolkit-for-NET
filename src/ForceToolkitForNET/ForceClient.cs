//TODO: add license header

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Reflection;
using Salesforce.Common;
using Salesforce.Common.Models;

namespace Salesforce.Force
{
    public class ForceClient : IForceClient, IDisposable
    {
        private ServiceHttpClient _serviceHttpClient;
        private const string UserAgent = "forcedotcom-toolkit-dotnet";
        
        public ForceClient(string instanceUrl, string accessToken, string apiVersion) 
            : this (instanceUrl, accessToken, apiVersion, new HttpClient())
        {
        }

        public ForceClient(string instanceUrl, string accessToken, string apiVersion, HttpClient httpClient)
        {
            if (string.IsNullOrEmpty(instanceUrl)) throw new ArgumentNullException("instanceUrl");
            if (string.IsNullOrEmpty(accessToken)) throw new ArgumentNullException("accessToken");
            if (string.IsNullOrEmpty(apiVersion)) throw new ArgumentNullException("apiVersion");
            if (httpClient == null) throw new ArgumentNullException("httpClient");

            //TODO: implement try/catch and throw auth exception if appropriate

            _serviceHttpClient = new ServiceHttpClient(instanceUrl, apiVersion, accessToken, UserAgent, httpClient);
        }

        public Task<QueryResult<T>> QueryAsync<T>(string query)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException("query");

            return _serviceHttpClient.HttpGetAsync<QueryResult<T>>(string.Format("query?q={0}", query));
        }

        public Task<QueryResult<T>> QueryContinuationAsync<T>(string nextRecordsUrl)
        {
            if (string.IsNullOrEmpty(nextRecordsUrl)) throw new ArgumentNullException("nextRecordsUrl");

            return _serviceHttpClient.HttpGetAsync<QueryResult<T>>(nextRecordsUrl);
        }

        public async Task<T> QueryByIdAsync<T>(string objectName, string recordId)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(recordId)) throw new ArgumentNullException("recordId");

            //TODO: implement try/catch and throw auth exception if appropriate

            var fields = string.Join(", ", typeof(T).GetRuntimeProperties().Select(p => p.Name));
            var query = string.Format("SELECT {0} FROM {1} WHERE Id = '{2}'", fields, objectName, recordId);
            var results = await QueryAsync<T>(query).ConfigureAwait(false);

            return results.records.FirstOrDefault();
        }

        public async Task<string> CreateAsync(string objectName, object record)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (record == null) throw new ArgumentNullException("record");

            //TODO: implement try/catch and throw auth exception if appropriate

            var response = await _serviceHttpClient.HttpPostAsync<SuccessResponse>(record, string.Format("sobjects/{0}", objectName)).ConfigureAwait(false);
            return response.id;
        }

        public Task<SuccessResponse> UpdateAsync(string objectName, string recordId, object record)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(recordId)) throw new ArgumentNullException("recordId");
            if (record == null) throw new ArgumentNullException("record");
            
            //TODO: implement try/catch and throw auth exception if appropriate

            return _serviceHttpClient.HttpPatchAsync(record, string.Format("sobjects/{0}/{1}", objectName, recordId));
        }

        public Task<SuccessResponse> UpsertExternalAsync(string objectName, string externalFieldName, string externalId, object record)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(externalFieldName)) throw new ArgumentNullException("externalFieldName");
            if (string.IsNullOrEmpty(externalId)) throw new ArgumentNullException("externalId");
            if (record == null) throw new ArgumentNullException("record");

            //TODO: implement try/catch and throw auth exception if appropriate

            return _serviceHttpClient.HttpPatchAsync(record, string.Format("sobjects/{0}/{1}/{2}", objectName, externalFieldName, externalId));
        }

        public Task<bool> DeleteAsync(string objectName, string recordId)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(recordId)) throw new ArgumentNullException("recordId");
            
            //TODO: implement try/catch and throw auth exception if appropriate

            return _serviceHttpClient.HttpDeleteAsync(string.Format("sobjects/{0}/{1}", objectName, recordId));
        }

        public Task<DescribeGlobalResult<T>> GetObjectsAsync<T>()
        {
            //TODO: implement try/catch and throw auth exception if appropriate

            return _serviceHttpClient.HttpGetAsync<DescribeGlobalResult<T>>("sobjects");
        }

        public Task<T> DescribeAsync<T>(string objectName)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            //TODO: implement try/catch and throw auth exception if appropriate

            return _serviceHttpClient.HttpGetAsync<T>(string.Format("sobjects/{0}", objectName));
        }

        public Task<T> RecentAsync<T>(int limit = 200)
        {
            //TODO: implement try/catch and throw auth exception if appropriate

            return _serviceHttpClient.HttpGetAsync<T>(string.Format("recent/?limit={0}", limit));
        }

        public async Task<T> UserInfo<T>(string accessToken, string uri)
        {
            if (string.IsNullOrEmpty(uri)) throw new ArgumentNullException("uri");
            //TODO: implement try/catch and throw auth exception if appropriate

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("access_token", accessToken)
            });

            var response = await _serviceHttpClient.HttpPostAsync<T>(content, new Uri(uri));
            return response;
        }

        public void Dispose()
        {
            _serviceHttpClient.Dispose();
        }
    }
}
