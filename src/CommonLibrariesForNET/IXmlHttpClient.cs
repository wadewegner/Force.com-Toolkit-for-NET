using System;
using System.Threading.Tasks;

namespace Salesforce.Common
{
    public interface IXmlHttpClient: IDisposable
    {
        // GET
        Task<T> HttpGetAsync<T>(string urlSuffix);
        Task<T> HttpGetAsync<T>(Uri uri);

        // POST
        Task<T> HttpPostAsync<T>(object inputObject, string urlSuffix);
        Task<T> HttpPostAsync<T>(object inputObject, Uri uri);
    }
}
