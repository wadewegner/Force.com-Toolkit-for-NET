using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Salesforce.Common;

namespace WebServerOAuthFlow.Controllers
{
    public class CallbackController : ApiController
    {
        private readonly string _consumerKey = ConfigurationSettings.AppSettings["ConsumerKey"];
        private readonly string _consumerSecret = ConfigurationSettings.AppSettings["ConsumerSecret"];
        private readonly string _callbackUrl = ConfigurationSettings.AppSettings["CallbackUrl"];
        private readonly string _tokenRequestEndpointUrl = ConfigurationSettings.AppSettings["TokenRequestEndpointUrl"];
        private const string UserAgent = "forcedotcom-toolkit-dotnet";

        public async Task<HttpResponseMessage> Get(string display, string code)
        {
            var auth = new AuthenticationClient();
            await auth.WebServerAsync(_consumerKey, _consumerSecret, _callbackUrl, code, UserAgent, _tokenRequestEndpointUrl);

            var url = string.Format("/?token={0}&api={1}&instance_url={2}", auth.AccessToken, auth.ApiVersion,
                auth.InstanceUrl);

            var response = new HttpResponseMessage(HttpStatusCode.Redirect);
            response.Headers.Location = new Uri(url, UriKind.Relative);

            return response;
        }
    }
}
