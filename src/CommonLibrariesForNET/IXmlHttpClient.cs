using System;
using System.Threading;
using System.Threading.Tasks;

namespace Salesforce.Common
{
    public interface IXmlHttpClient: IDisposable
    {
        // GET
        Task<T> HttpGetAsync<T>(string urlSuffix, CancellationToken token = default(CancellationToken));
        Task<T> HttpGetAsync<T>(Uri uri, CancellationToken token = default(CancellationToken));

        // POST
        Task<T> HttpPostAsync<T>(object inputObject, string urlSuffix, CancellationToken token = default(CancellationToken));
        Task<T> HttpPostAsync<T>(object inputObject, Uri uri, CancellationToken token = default(CancellationToken));
    }
}
