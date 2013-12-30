using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForceToolkitForNET
{
    //200	“OK” success code, for GET or HEAD request.
    //201	“Created” success code, for POST request.
    //204	“No Content” success code, for DELETE request.
    //300	The value returned when an external ID exists in more than one record. The response body contains the list of matching records.
    //304	The request content has not changed since a specified date and time. The date and time is provided in a If-Modified-Since header. See Get Object Metadata Changes for an example.
    //400	The request couldn’t be understood, usually because the JSON or XML body contains an error.
    //401	The session ID or OAuth token used has expired or is invalid. The response body contains the message and errorCode.
    //403	The request has been refused. Verify that the logged-in user has appropriate permissions.
    //404	The requested resource couldn’t be found. Check the URI for errors, and verify that there are no sharing issues.
    //405	The method specified in the Request-Line isn’t allowed for the resource specified in the URI.
    //415	The entity in the request is in a format that’s not supported by the specified method.
    //500	An error has occurred within Force.com, so the request couldn’t be completed. Contact salesforce.com Customer Support.


    public class ForceException : Exception
    {
        public ForceException(string error, string description) : base(description)
        {
            
        }

    }
}
