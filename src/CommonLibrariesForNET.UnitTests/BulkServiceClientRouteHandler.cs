using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Salesforce.Common.UnitTests
{
    internal class BulkServiceClientRouteHandler : DelegatingHandler
    {
        readonly Action<HttpRequestMessage> _testingAction;
        public readonly HttpResponseMessage Response;

        public BulkServiceClientRouteHandler(Action<HttpRequestMessage> testingAction, object response)
        {
            _testingAction = testingAction;
            Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new XmlContent(response)
            };
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            _testingAction(request);
            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(Response);
            return tsc.Task;
        }
    }
}
