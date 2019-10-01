using System.Net;

namespace Salesforce.Common.Models.Json 
{
    public interface IErrorResponse
    {
        ForceException MapToForceException(HttpStatusCode status);
    }
}
