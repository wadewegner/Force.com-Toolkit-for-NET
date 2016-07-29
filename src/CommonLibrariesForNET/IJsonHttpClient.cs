using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Salesforce.Common.Models.Json;

namespace Salesforce.Common
{
    public interface IJsonHttpClient: IDisposable
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
        Task<SuccessResponse> HttpPatchAsync(object inputObject, string urlSuffix);
        Task<SuccessResponse> HttpPatchAsync(object inputObject, Uri uri);

        // DELETE
        Task<bool> HttpDeleteAsync(string urlSuffix);
        Task<bool> HttpDeleteAsync(Uri uri);
    }
}