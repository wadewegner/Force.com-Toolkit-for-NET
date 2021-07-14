using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Salesforce.Common.Models.Json;
using Newtonsoft.Json;

namespace Salesforce.Common
{
    public interface IJsonHttpClient : IDisposable
    {

        // GET
        Task<T> HttpGetAsync<T>(string urlSuffix);
        Task<T> HttpGetAsync<T>(Uri uri);
        Task<IList<T>> HttpGetAsync<T>(string urlSuffix, string nodeName);
        Task<T> HttpGetRestApiAsync<T>(string apiName);

        // POST
        Task<T> HttpPostAsync<T>(object inputObject, string urlSuffix);
        Task<T> HttpPostAsync<T>(object inputObject, Uri uri);
        Task<T> HttpPostRestApiAsync<T>(string apiName, object inputObject);
        Task<T> HttpBinaryDataPostAsync<T>(string urlSuffix, object inputObject, byte[] fileContents, string headerName, string fileName);

        // PATCH
        Task<SuccessResponse> HttpPatchAsync(object inputObject, string urlSuffix, IDictionary<string, string> headers = default);
        Task<SuccessResponse> HttpPatchAsync(object inputObject, Uri uri, IDictionary<string, string> headers = default);
        Task<SuccessResponse> HttpPatchAsync(object inputObject, string urlSuffix, bool ignoreNull, IDictionary<string, string> headers = default);
        Task<SuccessResponse> HttpPatchAsync(object inputObject, Uri uri, NullValueHandling nullValueHandling, IDictionary<string, string> headers = default);

        // DELETE
        Task<bool> HttpDeleteAsync(string urlSuffix);
        Task<bool> HttpDeleteAsync(Uri uri);
    }
}