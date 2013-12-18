using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ForceSDKforNET
{
    internal class QueryResults<TRecord> where TRecord : new()
    {
        public List<TRecord> Records { get; set; }
    }

    public class ForceRestClient
    {
        public ForceRestClient()
        {
            ApiVersion = "v28.0";
        }

        public string ApiVersion { get; set; }
        public bool IsAuthenticated { get; private set; }
        public string InstanceUrl { get; private set; }
        public string AccessToken { get; private set; }

        public void Authenticate(string clientId, string clientSecret, string username, string password)
        {
            var tokenRequestEndpointUrl = "https://login.salesforce.com/services/oauth2/token";
            var client = new HttpClient();
            
            var content = new FormUrlEncodedContent(new[] 
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("grant_type", "password")
            });

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(tokenRequestEndpointUrl),
                Content = content
            };

            var responseMessage = client.SendAsync(request).Result;
            var response = responseMessage.Content.ReadAsStringAsync();
            var authToken = JsonConvert.DeserializeObject<AuthToken>(response.Result);

            AccessToken = authToken.access_token;
            InstanceUrl = authToken.instance_url;
        }

        public IList<T> Query<T>(string query)
        {

            var url = string.Format("{0}?q={1}", GetUrl("query"), query);

            var client = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };

            request.Headers.Add("Authorization", "Bearer " + AccessToken);

            var responseMessage = client.SendAsync(request).Result;
            var response = responseMessage.Content.ReadAsStringAsync();

            var jObject = JObject.Parse(response.Result);
            var jToken = jObject.GetValue("records");

            var r = JsonConvert.DeserializeObject<IList<T>>(jToken.ToString());
            return r;
        }


        protected string GetUrl(string resourceName)
        {
            return string.Format("{0}/services/data/{1}/{2}", InstanceUrl, ApiVersion, resourceName);
        }
    }




}
