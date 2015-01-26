using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Salesforce.Force.UnitTests
{
    public class FakeHttpRequestHandler : DelegatingHandler
    {
        readonly HttpResponseMessage _expectedResponse;

        public FakeHttpRequestHandler(HttpResponseMessage expectedResponse)
        {
            _expectedResponse = expectedResponse;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(_expectedResponse);
            return tsc.Task;
        }
    }
}
