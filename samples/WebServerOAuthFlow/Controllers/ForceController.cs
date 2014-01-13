using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Salesforce.Common;
using Salesforce.Force;

namespace WebServerOAuthFlow.Controllers
{
    public class ForceController : ApiController
    {
        public async Task<string> Get([FromUri] string instanceUrl, [FromUri] string accessToken, [FromUri] string apiVersion, [FromUri] string query)
        {
            var client = new ForceClient(instanceUrl, accessToken, apiVersion);
            var queryResults = await client.Query<object>(query);
            var response = JsonConvert.SerializeObject(queryResults);

            return response;
        }

    }
}