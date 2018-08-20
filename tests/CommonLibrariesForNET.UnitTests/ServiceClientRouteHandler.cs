using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Salesforce.Common.UnitTests
{
    internal class ServiceClientRouteHandler : DelegatingHandler
    {
        readonly Action<HttpRequestMessage> _testingAction;

        public ServiceClientRouteHandler(Action<HttpRequestMessage> testingAction)
        {
            _testingAction = testingAction;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            _testingAction(request);
            var resp = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new JsonContent(new
                {
                    node = new JsonContent(new
                    {
                        Success = true,
                        Message = "Success"
                    })
                })
            };

            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(resp);
            return tsc.Task;
        }
    }
}