//TODO: add license header

using System.Threading.Tasks;

namespace Salesforce.Common
{
    interface IAuthenticationClient
    {
        string InstanceUrl { get; set; }
        string AccessToken { get; set; }
        string ApiVersion { get; set; }
        Task UsernamePasswordAsync(string clientId, string clientSecret, string username, string password);
        Task UsernamePasswordAsync(string clientId, string clientSecret, string username, string password, string userAgent);
        Task UsernamePasswordAsync(string clientId, string clientSecret, string username, string password, string userAgent, string tokenRequestEndpointUrl);
        Task WebServerAsync(string clientId, string clientSecret, string redirectUri, string code, string state);
        Task WebServerAsync(string clientId, string clientSecret, string redirectUri, string code, string state, string userAgent);
        Task WebServerAsync(string clientId, string clientSecret, string redirectUri, string code, string state, string userAgent, string tokenRequestEndpointUrl);
        void Dispose();
    }
}
