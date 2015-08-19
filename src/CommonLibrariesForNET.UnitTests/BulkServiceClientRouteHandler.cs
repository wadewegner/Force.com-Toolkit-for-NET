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
        private readonly object _response;

        public BulkServiceClientRouteHandler(Action<HttpRequestMessage> testingAction, object response)
        {
            _testingAction = testingAction;
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            _testingAction(request);
            var resp = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new XmlContent(_response)
            };

            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(resp);
            return tsc.Task;
        }
    }
}
