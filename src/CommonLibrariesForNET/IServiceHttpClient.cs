using System.Threading.Tasks;
using Salesforce.Common.Models;
using System.Net.Http;
using System;

namespace Salesforce.Common
{
    public interface IServiceHttpClient
    {
        Task<T> HttpGetAsync<T>(string urlSuffix, string parameters=null);
        Task<T> HttpGetAsync<T>(HttpRequestMessage request);
        Task<T> HttpGetAsync<T>(Uri url);
        Task<T> HttpPostApexRestAsync<T>(string urlSuffix, object inputObject);
        Task<T> HttpGetApexRestAsync<T>(string apiName, string parameters);
        Task<T> HttpPostAsync<T>(object inputObject, string urlSuffix);
        Task<T> HttpPostAsync<T>(object inputObject, Uri uri);
        Task<T> HttpPostAsync<T>(string json, Uri uri);
        Task<SuccessResponse> HttpPatchAsync(object inputObject, string urlSuffix);
        Task<bool> HttpDeleteAsync(string urlSuffix);
        void Dispose();
    }
}