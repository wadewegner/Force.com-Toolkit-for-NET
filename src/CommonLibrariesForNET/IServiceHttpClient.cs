using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Salesforce.Common.Models;

namespace Salesforce.Common
{
    public interface IServiceHttpClient : IDisposable
    {
        Task<T> HttpGetAsync<T>(string urlSuffix);
        Task<T> HttpGetRestApiAsync<T>(string apiName);
        Task<IList<T>> HttpGetAsync<T>(string urlSuffix, string nodeName);
        Task<T> HttpGetAsync<T>(Uri uri);
        Task<T> HttpPostRestApiAsync<T>(string apiName, object inputObject);
        Task<T> HttpPostAsync<T>(object inputObject, string urlSuffix);
        Task<T> HttpPostAsync<T>(object inputObject, Uri uri);
        Task<SuccessResponse> HttpPatchAsync(object inputObject, string urlSuffix);
        Task<bool> HttpDeleteAsync(string urlSuffix);
        Task<T> HttpBinaryDataPostAsync<T>(string urlSuffix, object inputObject, byte[] fileContents, string headerName, string fileName);
    }
}