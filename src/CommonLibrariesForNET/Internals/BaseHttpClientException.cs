using System;
using System.Net;

namespace Salesforce.Common.Internals
{
    internal sealed class BaseHttpClientException : Exception
    {
        private readonly HttpStatusCode _httpStatusCode;

        internal BaseHttpClientException(string response, HttpStatusCode statusCode) : base(response)
        {
            _httpStatusCode = statusCode;
        }

        internal HttpStatusCode GetStatus()
        {
            return _httpStatusCode;
        }
    }
}
