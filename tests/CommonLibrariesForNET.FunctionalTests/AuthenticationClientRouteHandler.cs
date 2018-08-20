using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Salesforce.Common.Models.Json;

namespace Salesforce.Common.UnitTests
{
    internal class AuthenticationClientRouteHandler : DelegatingHandler
    {
        readonly Action<HttpRequestMessage> _testingAction;

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
                    AccessToken = "AccessToken",
                    Id = "Id",
                    InstanceUrl = "InstanceUrl",
                    IssuedAt = "IssuedAt",
                    Signature = "Signature"
                })
            };

            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(resp);
            return tsc.Task;
        }
    }
}