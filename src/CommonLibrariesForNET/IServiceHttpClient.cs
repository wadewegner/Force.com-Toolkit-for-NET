using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Salesforce.Common.Models;

namespace Salesforce.Common
{
    public interface IServiceHttpClient
    {
        void Dispose();
        Task<T> HttpGetAsync<T>(string urlSuffix);
        Task<T> HttpGetRestApiAsync<T>(string apiName);
        Task<IList<T>> HttpGetAsync<T>(string urlSuffix, string nodeName);
        Task<T> HttpGetAsync<T>(Uri uri);
        Task<T> HttpPostRestApiAsync<T>(string apiName, object inputObject);

        Task<T> HttpPostAsync<T>(object inputObject, string urlSuffix);
        Task<T> HttpPostAsync<T>(object inputObject, string urlSuffix, Dictionary<string, string> headers);
        Task<T> HttpPostAsync<T>(object inputObject, Uri uri);
        Task<T> HttpPostAsync<T>(object inputObject, Uri uri, Dictionary<string, string> headers);
        
        Task<SuccessResponse> HttpPatchAsync(object inputObject, string urlSuffix);
        Task<SuccessResponse> HttpPatchAsync(object inputObject, string urlSuffix, Dictionary<string, string> headers);
        
        Task<bool> HttpDeleteAsync(string urlSuffix);
        Task<T> HttpBinaryDataPostAsync<T>(string urlSuffix, object inputObject, byte[] fileContents, string headerName, string fileName);
    }
}