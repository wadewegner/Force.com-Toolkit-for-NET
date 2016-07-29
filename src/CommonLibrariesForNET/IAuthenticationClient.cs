using System;
using System.Threading.Tasks;

namespace Salesforce.Common
{
    public interface IAuthenticationClient : IDisposable
    {
        string InstanceUrl { get; set; }
        string AccessToken { get; set; }
        string ApiVersion { get; set; }
        Task UsernamePasswordAsync(string clientId, string clientSecret, string username, string password);
        Task UsernamePasswordAsync(string clientId, string clientSecret, string username, string password, string tokenRequestEndpointUrl);
        Task WebServerAsync(string clientId, string clientSecret, string redirectUri, string code);
        Task WebServerAsync(string clientId, string clientSecret, string redirectUri, string code, string tokenRequestEndpointUrl);
    }
}
