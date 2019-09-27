using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Salesforce.Common.Models.Json
{
    public class ErrorResponses : List<ErrorResponse>, IErrorResponse
    {
        public ForceException MapToForceException(HttpStatusCode status)
        {
            return new ForceException(this.FirstOrDefault()?.ErrorCode, this.FirstOrDefault()?.Message);
        }
    }
}
