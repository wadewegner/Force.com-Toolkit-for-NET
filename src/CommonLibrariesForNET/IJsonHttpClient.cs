using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Salesforce.Common.Models.Json;

namespace Salesforce.Common
{
    public interface IJsonHttpClient: IDisposable
    {

        // GET
        Task<T> HttpGetAsync<T>(string urlSuffix, CancellationToken token = default(CancellationToken));
        Task<T> HttpGetAsync<T>(Uri uri, CancellationToken token = default(CancellationToken));
        Task<IList<T>> HttpGetAsync<T>(string urlSuffix, string nodeName, CancellationToken token = default(CancellationToken));
		Task<T> HttpGetRestApiAsync<T>(string apiName, CancellationToken token = default(CancellationToken));

        // POST
        Task<T> HttpPostAsync<T>(object inputObject, string urlSuffix, CancellationToken token = default(CancellationToken));
        Task<T> HttpPostAsync<T>(object inputObject, Uri uri, CancellationToken token = default(CancellationToken));
        Task<T> HttpPostRestApiAsync<T>(string apiName, object inputObject, CancellationToken token = default(CancellationToken));
        Task<T> HttpBinaryDataPostAsync<T>(string urlSuffix, object inputObject, byte[] fileContents, string headerName, string fileName, CancellationToken token = default(CancellationToken));

        // PATCH
        Task<SuccessResponse> HttpPatchAsync(object inputObject, string urlSuffix, CancellationToken token = default(CancellationToken));
        Task<SuccessResponse> HttpPatchAsync(object inputObject, Uri uri, CancellationToken token = default(CancellationToken));

        // DELETE
        Task<bool> HttpDeleteAsync(string urlSuffix, CancellationToken token = default(CancellationToken));
        Task<bool> HttpDeleteAsync(Uri uri, CancellationToken token = default(CancellationToken));
    }
}