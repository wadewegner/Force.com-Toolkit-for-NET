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
    public class ForceClient : IForceClient
    {
        private static ServiceHttpClient _serviceHttpClient;
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

        public async Task<QueryResult<T>> QueryAsync<T>(string query)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException("query");
            
            //TODO: implement try/catch and throw auth exception if appropriate

            var response = await _serviceHttpClient.HttpGetAsync<QueryResult<T>>(string.Format("query?q={0}", query));
            return response;
        }

        public async Task<T> QueryByIdAsync<T>(string objectName, string recordId)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(recordId)) throw new ArgumentNullException("recordId");

            //TODO: implement try/catch and throw auth exception if appropriate

            var fields = string.Join(", ", typeof(T).GetRuntimeProperties().Select(p => p.Name));
            var query = string.Format("SELECT {0} FROM {1} WHERE Id = '{2}'", fields, objectName, recordId);
            var results = await QueryAsync<T>(query);

            return results.records.FirstOrDefault();
        }

        public async Task<string> CreateAsync(string objectName, object record)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (record == null) throw new ArgumentNullException("record");

            //TODO: implement try/catch and throw auth exception if appropriate

            var response = await _serviceHttpClient.HttpPostAsync<SuccessResponse>(record, string.Format("sobjects/{0}", objectName));
            return response.id;
        }

        public async Task<bool> UpdateAsync(string objectName, string recordId, object record)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(recordId)) throw new ArgumentNullException("recordId");
            if (record == null) throw new ArgumentNullException("record");
            
            //TODO: implement try/catch and throw auth exception if appropriate

            var response = await _serviceHttpClient.HttpPatchAsync(record, string.Format("sobjects/{0}/{1}", objectName, recordId));
            return response;
        }

        public async Task<bool> UpsertExternalAsync(string objectName, string externalId, string recordId, object record)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(externalId)) throw new ArgumentNullException("externalId");
            if (string.IsNullOrEmpty(recordId)) throw new ArgumentNullException("recordId");
            if (record == null) throw new ArgumentNullException("record");

            //TODO: implement try/catch and throw auth exception if appropriate

            var response = await _serviceHttpClient.HttpPatchAsync(record, string.Format("sobjects/{0}/{1}/{2}", objectName, externalId, recordId));
            return response;
        }

        public async Task<bool> DeleteAsync(string objectName, string recordId)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(recordId)) throw new ArgumentNullException("recordId");
            
            //TODO: implement try/catch and throw auth exception if appropriate

            var response = await _serviceHttpClient.HttpDeleteAsync(string.Format("sobjects/{0}/{1}", objectName, recordId));
            return response;
        }

        public async Task<DescribeGlobalResult<T>> GetObjectsAsync<T>()
        {
            //TODO: implement try/catch and throw auth exception if appropriate

            var response = await _serviceHttpClient.HttpGetAsync<DescribeGlobalResult<T>>("sobjects");
            return response;
        }

        public async Task<T> DescribeAsync<T>(string objectName)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            //TODO: implement try/catch and throw auth exception if appropriate

            var response = await _serviceHttpClient.HttpGetAsync<T>(string.Format("sobjects/{0}", objectName));
            return response;
        }

        public async Task<T> RecentAsync<T>(int limit = 200)
        {
            //TODO: implement try/catch and throw auth exception if appropriate

            var response = await _serviceHttpClient.HttpGetAsync<T>(string.Format("recent/?limit={0}", limit));
            return response;
        }
    }
}
