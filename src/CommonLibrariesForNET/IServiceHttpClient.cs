using System.Threading.Tasks;
using Salesforce.Common.Models;

namespace Salesforce.Common
{
    public interface IServiceHttpClient
    {
        Task<T> HttpGetAsync<T>(string urlSuffix);
		Task<T> HttpGetRestApiAsync<T>(string apiName, string parameters);
        Task<T> HttpPostAsync<T>(object inputObject, string urlSuffix);
        Task<SuccessResponse> HttpPatchAsync(object inputObject, string urlSuffix);
        Task<bool> HttpDeleteAsync(string urlSuffix);
        void Dispose();
    }
}