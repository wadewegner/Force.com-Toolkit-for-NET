using System;
using System.Threading;
using System.Threading.Tasks;

namespace Salesforce.Common
{
    public interface IAuthenticationClient : IDisposable
    {
        string InstanceUrl { get; set; }
        string AccessToken { get; set; }
        string ApiVersion { get; set; }
	    string Id { get; set; }
		Task UsernamePasswordAsync(string clientId, string clientSecret, string username, string password, CancellationToken token = default(CancellationToken));
        Task UsernamePasswordAsync(string clientId, string clientSecret, string username, string password, string tokenRequestEndpointUrl, CancellationToken token = default(CancellationToken));
        Task WebServerAsync(string clientId, string clientSecret, string redirectUri, string code, CancellationToken token = default(CancellationToken));
        Task WebServerAsync(string clientId, string clientSecret, string redirectUri, string code, string tokenRequestEndpointUrl, CancellationToken token = default(CancellationToken));
    }
}
