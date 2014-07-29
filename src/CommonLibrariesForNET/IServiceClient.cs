//TODO: add license header

using System.Threading.Tasks;
using Salesforce.Common.Models;

namespace Salesforce.Common
{
    public interface IServiceHttpClient
    {
        Task<T> HttpGetAsync<T>(string urlSuffix);
        //Task<T> HttpGetAsync<T>(string urlSuffix, string nodeName);
        Task<T> HttpPostAsync<T>(object inputObject, string urlSuffix);
        Task<SuccessResponse> HttpPatchAsync(object inputObject, string urlSuffix);
        Task<bool> HttpDeleteAsync(string urlSuffix);
        void Dispose();
    }
}