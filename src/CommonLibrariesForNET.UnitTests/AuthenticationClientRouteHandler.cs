//TODO: add license header

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Salesforce.Common.Models;

namespace Salesforce.Common.UnitTests
{
    internal class AuthenticationClientRouteHandler : DelegatingHandler
    {
        Action<HttpRequestMessage> _testingAction;

        public AuthenticationClientRouteHandler(Action<HttpRequestMessage> testingAction)
        {
            _testingAction = testingAction;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            _testingAction(request);

            var resp = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new JsonContent(new AuthToken
                {
                    access_token = "access_token",
                    id = "id",
                    instance_url = "instance_url",
                    issued_at = "issued_at",
                    signature = "signature"
                })
            };

            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(resp);
            return tsc.Task;
        }
    }
}