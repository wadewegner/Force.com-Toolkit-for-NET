using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Salesforce.Force.UnitTests
{
    public class BulkFakeHttpRequestHandler : DelegatingHandler
    {

        readonly List<Action<HttpRequestMessage>> _testingActions;
        readonly List<HttpResponseMessage> _expectedResponses;
        private int _callNum;

        public BulkFakeHttpRequestHandler(List<HttpResponseMessage> expectedResponses, List<Action<HttpRequestMessage>> testingActions)
        {
            _expectedResponses = expectedResponses;
            _testingActions = testingActions;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            _testingActions[_callNum](request);
            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(_expectedResponses[_callNum]);
            _callNum++;
            return tsc.Task;
        }
    }
}
