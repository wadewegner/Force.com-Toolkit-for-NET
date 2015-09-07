using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Salesforce.Common.Models;

namespace Salesforce.Force
{
    public interface IForceClient
    {
        Task<QueryResult<T>> QueryAsync<T>(string query);
        Task<QueryResult<T>> QueryContinuationAsync<T>(string nextRecordsUrl);
        Task<QueryResult<T>> QueryAllAsync<T>(string query);
        Task<T> QueryByIdAsync<T>(string objectName, string recordId);
        Task<T> ExecuteRestApiAsync<T>(string apiName);
        Task<T> ExecuteRestApiAsync<T>(string apiName, object inputObject);
        Task<SuccessResponse> CreateAsync(string objectName, object record);
        Task<SuccessResponse> UpdateAsync(string objectName, string recordId, object record);
        Task<SuccessResponse> UpsertExternalAsync(string objectName, string externalFieldName, string externalId, object record);
        Task<bool> DeleteAsync(string objectName, string recordId);
        Task<bool> DeleteExternalAsync(string objectName, string externalFieldName, string externalId);
        Task<DescribeGlobalResult<T>> GetObjectsAsync<T>();
        Task<T> BasicInformationAsync<T>(string objectName);
        Task<T> DescribeAsync<T>(string objectName);
        Task<T> GetDeleted<T>(string objectName, DateTime startDateTime, DateTime endDateTime);
        Task<T> GetUpdated<T>(string objectName, DateTime startDateTime, DateTime endDateTime);
        Task<T> DescribeLayoutAsync<T>(string objectName);
        Task<T> DescribeLayoutAsync<T>(string objectName, string recordTypeId);
        Task<T> RecentAsync<T>(int limit = 200);
        Task<List<T>> SearchAsync<T>(string query);
        Task<T> UserInfo<T>(string url);
        void Dispose();
    }
}
