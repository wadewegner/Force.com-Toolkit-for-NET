//TODO: add license header

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Salesforce.Common.Models;
using Newtonsoft.Json;

namespace Salesforce.Common
{
    public class AuthenticationClient : IAuthenticationClient, IDisposable
    {
        public string InstanceUrl { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Id { get; set; }
        public string ApiVersion { get; set; }

        private const string UserAgent = "common-libraries-dotnet";
        private const string TokenRequestEndpointUrl = "https://login.salesforce.com/services/oauth2/token";
        private HttpClient _httpClient;

        public AuthenticationClient()
            : this(new HttpClient())
        {
        }

        public AuthenticationClient(HttpClient httpClient)
        {
            if (httpClient == null) throw new ArgumentNullException("httpClient");

            _httpClient = httpClient;
            ApiVersion = "v29.0";
        }

        public async Task UsernamePasswordAsync(string clientId, string clientSecret, string username, string password)
        {
            await UsernamePasswordAsync(clientId, clientSecret, username, password, UserAgent, TokenRequestEndpointUrl);
        }

        public async Task UsernamePasswordAsync(string clientId, string clientSecret, string username, string password, string userAgent)
        {
            await UsernamePasswordAsync(clientId, clientSecret, username, password, userAgent, TokenRequestEndpointUrl);
        }

        public async Task UsernamePasswordAsync(string clientId, string clientSecret, string username, string password, string userAgent, string tokenRequestEndpointUrl)
        {
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException("clientId");
            if (string.IsNullOrEmpty(clientSecret)) throw new ArgumentNullException("clientSecret");
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException("username");
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException("password");
            if (string.IsNullOrEmpty(userAgent)) throw new ArgumentNullException("userAgent");
            if (string.IsNullOrEmpty(tokenRequestEndpointUrl)) throw new ArgumentNullException("tokenRequestEndpointUrl");
            //TODO: check to make sure tokenRequestEndpointUrl is a valid URI

            var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password)
                });

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(tokenRequestEndpointUrl),
                Content = content
            };

            request.Headers.UserAgent.ParseAdd(string.Concat(userAgent, "/", ApiVersion));

            var responseMessage = await _httpClient.SendAsync(request);
            var response = await responseMessage.Content.ReadAsStringAsync();

            if (responseMessage.IsSuccessStatusCode)
            {
                var authToken = JsonConvert.DeserializeObject<AuthToken>(response);

                AccessToken = authToken.access_token;
                InstanceUrl = authToken.instance_url;
                Id = authToken.id;
            }
            else
            {
                var errorResponse = JsonConvert.DeserializeObject<AuthErrorResponse>(response);
                throw new ForceAuthException(errorResponse.error, errorResponse.error_description);
            }
        }

        public async Task WebServerAsync(string clientId, string clientSecret, string redirectUri, string code)
        {
            await WebServerAsync(clientId, clientSecret, redirectUri, code, UserAgent, TokenRequestEndpointUrl);
        }

        public async Task WebServerAsync(string clientId, string clientSecret, string redirectUri, string code, string userAgent)
        {
            await WebServerAsync(clientId, clientSecret, redirectUri, code, userAgent, TokenRequestEndpointUrl);
        }

        public async Task<IdentityResponse> IdendificationService(string access_token, string id)
        {
            if (access_token == null)
            {
                if (AccessToken == null)
                {
                    throw new ForceException("No access token", "The AuthenticationClient has not authenticated the user, and no access token exists. Can not make a call to the Identity Service without first authenticating.");
                }
                else
                {
                    access_token = AccessToken;
                }
            }

            var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("access_token", access_token)
                });

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(id),
                Content = content
            };
            request.Headers.UserAgent.ParseAdd(string.Concat("common-libraries-dotnet", "/", ApiVersion));
            var responseMessage = await _httpClient.SendAsync(request);
            var response = await responseMessage.Content.ReadAsStringAsync();
            if (responseMessage.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<IdentityResponse>(response);
            }
            else
            {
                var errorResponse = JsonConvert.DeserializeObject<AuthErrorResponse>(response);
                throw new ForceAuthException(errorResponse.error, errorResponse.error_description);
            }
        }

        public async Task WebServerAsync(string clientId, string clientSecret, string redirectUri, string code, string userAgent, string tokenRequestEndpointUrl)
        {
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException("clientId");
            if (string.IsNullOrEmpty(clientSecret)) throw new ArgumentNullException("clientSecret");
            if (string.IsNullOrEmpty(redirectUri)) throw new ArgumentNullException("redirectUri");
            //TODO: check to make sure redirectUri is a valid URI
            if (string.IsNullOrEmpty(code)) throw new ArgumentNullException("code");
            if (string.IsNullOrEmpty(userAgent)) throw new ArgumentNullException("userAgent");
            if (string.IsNullOrEmpty(tokenRequestEndpointUrl)) throw new ArgumentNullException("tokenRequestEndpointUrl");
            //TODO: check to make sure tokenRequestEndpointUrl is a valid URI

            var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri),
                    new KeyValuePair<string, string>("code", code)
                });

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(tokenRequestEndpointUrl),
                Content = content
            };

            request.Headers.UserAgent.ParseAdd(string.Concat(userAgent, "/", ApiVersion));

            var responseMessage = await _httpClient.SendAsync(request);
            var response = await responseMessage.Content.ReadAsStringAsync();

            if (responseMessage.IsSuccessStatusCode)
            {
                var authToken = JsonConvert.DeserializeObject<AuthToken>(response);

                AccessToken = authToken.access_token;
                InstanceUrl = authToken.instance_url;
                Id = authToken.id;
                RefreshToken = authToken.refresh_token;
            }
            else
            {
                var errorResponse = JsonConvert.DeserializeObject<AuthErrorResponse>(response);
                throw new ForceAuthException(errorResponse.error, errorResponse.error_description);
            }
        }

        public async Task TokenRefreshAsync(string clientId, string refreshToken, string clientSecret = "")
        {
            await TokenRefreshAsync(clientId, refreshToken, clientSecret, UserAgent, TokenRequestEndpointUrl);
        }

        public async Task TokenRefreshAsync(string clientId, string refreshToken, string clientSecret, string userAgent)
        {
            await TokenRefreshAsync(clientId, refreshToken, clientSecret, userAgent, TokenRequestEndpointUrl);
        }

        public async Task TokenRefreshAsync(string clientId, string refreshToken, string clientSecret, string userAgent, string tokenRequestEndpointUrl)
        {
            var url = Common.FormatRefreshTokenUrl(
                tokenRequestEndpointUrl,
                clientId,
                refreshToken,
                clientSecret);

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url)
            };

            request.Headers.UserAgent.ParseAdd(string.Concat(userAgent, "/", ApiVersion));

            var responseMessage = await _httpClient.SendAsync(request);
            var response = await responseMessage.Content.ReadAsStringAsync();

            if (responseMessage.IsSuccessStatusCode)
            {
                var authToken = JsonConvert.DeserializeObject<AuthToken>(response);

                AccessToken = authToken.access_token;
                InstanceUrl = authToken.instance_url;
                Id = authToken.id;
            }
            else
            {
                var errorResponse = JsonConvert.DeserializeObject<AuthErrorResponse>(response);
                throw new ForceException(errorResponse.error, errorResponse.error_description);
            }
        }

        public void Dispose()
        {
            //TODO: catch in case this has already been disposed or deallocated?
            _httpClient.Dispose();
        }
    }
}
