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
        Task<T> HttpGetAsync<T, TErrorResponse>(string urlSuffix) where TErrorResponse : IErrorResponse;
        Task<T> HttpGetAsync<T>(Uri uri);
        Task<T> HttpGetAsync<T, TErrorResponse>(Uri uri) where TErrorResponse : IErrorResponse;
        Task<IList<T>> HttpGetAsync<T>(string urlSuffix, string nodeName);
        Task<IList<T>> HttpGetAsync<T, TErrorResponse>(string urlSuffix, string nodeName) where TErrorResponse : IErrorResponse;
        Task<T> HttpGetRestApiAsync<T>(string apiName);
        Task<T> HttpGetRestApiAsync<T, TErrorResponse>(string apiName) where TErrorResponse : IErrorResponse;

        // POST
        Task<T> HttpPostAsync<T>(object inputObject, string urlSuffix);
        Task<T> HttpPostAsync<T, TErrorResponse>(object inputObject, string urlSuffix) where TErrorResponse : IErrorResponse;
        Task<T> HttpPostAsync<T>(object inputObject, Uri uri);
        Task<T> HttpPostAsync<T, TErrorResponse>(object inputObject, Uri uri) where TErrorResponse : IErrorResponse;
        Task<T> HttpPostRestApiAsync<T>(string apiName, object inputObject);
        Task<T> HttpPostRestApiAsync<T, TErrorResponse>(string apiName, object inputObject) where TErrorResponse : IErrorResponse;
        Task<T> HttpBinaryDataPostAsync<T>(string urlSuffix, object inputObject, byte[] fileContents, string headerName, string fileName);
        Task<T> HttpBinaryDataPostAsync<T, TErrorResponse>(string urlSuffix, object inputObject, byte[] fileContents, string headerName, string fileName) where TErrorResponse : IErrorResponse;

        // PATCH
        Task<SuccessResponse> HttpPatchAsync(object inputObject, string urlSuffix);
        Task<SuccessResponse> HttpPatchAsync(object inputObject, Uri uri);
        Task<SuccessResponse> HttpPatchAsync(object inputObject, string urlSuffix, bool ignoreNull);
        Task<SuccessResponse> HttpPatchAsync(object inputObject, Uri uri, NullValueHandling nullValueHandling);

        // DELETE
        Task<bool> HttpDeleteAsync(string urlSuffix);
        Task<bool> HttpDeleteAsync(Uri uri);
    }
}