using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CommonToolkitForNET;
using CommonToolkitForNET.Models;
using Newtonsoft.Json;

namespace ForceToolkitForNET
{
    public class ToolkitHttpClientTest
    {
        private readonly string _instanceUrl;
        private readonly string _apiVersion;
        private readonly string _userAgent;
        private readonly string _accessToken;
        public ToolkitHttpClientTest(string instanceUrl, string apiVersion, string accessToken)
        {
            _instanceUrl = instanceUrl;
            _apiVersion = apiVersion;
            _accessToken = accessToken;
            _userAgent = "common-toolkit-dotnet";

        }

        public ToolkitHttpClientTest(string instanceUrl, string apiVersion, string accessToken, string userAgent)
        {
            _instanceUrl = instanceUrl;
            _apiVersion = apiVersion;
            _accessToken = accessToken;
            _userAgent = userAgent;
        }

        
    }
}
