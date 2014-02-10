//TODO: add license header

using System;
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
        private const string UserAgent = "forcedotcom-libraries-dotnet";

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
      
        //public async Task<IList<T>> Query<T>(string query)
        //{
        //    var response = await _serviceHttpClient.HttpGet<IList<T>>(string.Format("query?q={0}", query), "records");
        //    return response;
        //}

        public async Task<T> Query<T>(string query)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException("query");
            
            //TODO: implement try/catch and throw auth exception if appropriate

            var response = await _serviceHttpClient.HttpGet<T>(string.Format("query?q={0}", query), "records");
            return response;
        }

        public async Task<T> QueryById<T>(string objectName, string recordId)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(recordId)) throw new ArgumentNullException("recordId");

            //TODO: implement try/catch and throw auth exception if appropriate

            var fields = string.Join(", ", typeof(T).GetProperties().Select(p => p.Name));
            var query = string.Format("SELECT {0} FROM {1} WHERE Id = '{2}'", fields, objectName, recordId);
            var results = await Query<T>(query);

            return ((IList<T>)(results)).FirstOrDefault();
        }

        public async Task<string> Create(string objectName, object record)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (record == null) throw new ArgumentNullException("record");

            //TODO: implement try/catch and throw auth exception if appropriate

            var response = await _serviceHttpClient.HttpPost<SuccessResponse>(record, string.Format("sobjects/{0}", objectName));
            return response.id;
        }

        public async Task<bool> Update(string objectName, string recordId, object record)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(recordId)) throw new ArgumentNullException("recordId");
            if (record == null) throw new ArgumentNullException("record");
            
            //TODO: implement try/catch and throw auth exception if appropriate

            var response = await _serviceHttpClient.HttpPatch(record, string.Format("sobjects/{0}/{1}", objectName, recordId));
            return response;
        }

        public async Task<bool> Delete(string objectName, string recordId)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(recordId)) throw new ArgumentNullException("recordId");
            
            //TODO: implement try/catch and throw auth exception if appropriate

            var response = await _serviceHttpClient.HttpDelete(string.Format("sobjects/{0}/{1}", objectName, recordId));
            return response;
        }

        //TODO: Does this actually need to change?
        //public async Task<IList<T>> GetObjects<T>()
        //{
        //    var response = await _serviceHttpClient.HttpGet<IList<T>>("sobjects", "sobjects");
        //    return response;
        //}

        public async Task<T> GetObjects<T>()
        {
            //TODO: implement try/catch and throw auth exception if appropriate

            var response = await _serviceHttpClient.HttpGet<T>("sobjects", "sobjects");
            return response;
        }

        public async Task<T> Describe<T>(string objectName)
        {
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentNullException("objectName");
            
            //TODO: implement try/catch and throw auth exception if appropriate

            var response = await _serviceHttpClient.HttpGet<T>(string.Format("sobjects/{0}", objectName), "objectDescribe");
            return response;
        }

        public async Task<T> Recent<T>(int limit = 200)
        {
            //TODO: implement try/catch and throw auth exception if appropriate

            var response = await _serviceHttpClient.HttpGet<T>(string.Format("recent/?limit={0}", limit));
            return response;
        }
    }
}
