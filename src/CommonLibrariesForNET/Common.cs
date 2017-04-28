using System;
using Salesforce.Common.Models.Json;

namespace Salesforce.Common
{
    public static class Common
    {
        public static Uri FormatUrl(string resourceName, string instanceUrl, string apiVersion)
        {
            if (string.IsNullOrEmpty(resourceName)) throw new ArgumentNullException("resourceName");
            if (string.IsNullOrEmpty(instanceUrl)) throw new ArgumentNullException("instanceUrl");
            if (string.IsNullOrEmpty(apiVersion)) throw new ArgumentNullException("apiVersion");

            if (resourceName.StartsWith("/services/data", StringComparison.CurrentCultureIgnoreCase))
            {
                return new Uri(new Uri(instanceUrl), resourceName);
            }

	        if (resourceName.StartsWith("/services/async", StringComparison.CurrentCultureIgnoreCase))
            {
                return new Uri(new Uri(instanceUrl), string.Format(resourceName, apiVersion));
            }

            return new Uri(new Uri(instanceUrl), string.Format("/services/data/{0}/{1}", apiVersion, resourceName));
        }
        
        public static Uri FormatCustomUrl(string customApi, string parameters, string instanceUrl)
        {
            if (string.IsNullOrEmpty(customApi)) throw new ArgumentNullException("customApi");
            if (string.IsNullOrEmpty(parameters)) throw new ArgumentNullException("parameters");
            if (string.IsNullOrEmpty(instanceUrl)) throw new ArgumentNullException("instanceUrl");

            return new Uri(string.Format("{0}/services/apexrest/{1}{2}", instanceUrl, customApi, parameters));
        }

        public static Uri FormatRestApiUrl(string customApi, string instanceUrl)
        {
            if (string.IsNullOrEmpty(customApi)) throw new ArgumentNullException("customApi");
            if (string.IsNullOrEmpty(instanceUrl)) throw new ArgumentNullException("instanceUrl");

            return new Uri(string.Format("{0}/services/apexrest/{1}", instanceUrl, customApi));
        }
        
		public static string FormatAuthUrl(
            string loginUrl,
            ResponseTypes responseType,
            string clientId,
            string redirectUrl,
            DisplayTypes display = DisplayTypes.Page,
            bool immediate = false,
            string state = "",
            string scope = "")
        {
            if (string.IsNullOrEmpty(loginUrl)) throw new ArgumentNullException("loginUrl");
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException("clientId");
            if (string.IsNullOrEmpty(redirectUrl)) throw new ArgumentNullException("redirectUrl");

            var url =
            string.Format(
                "{0}?response_type={1}&client_id={2}&redirect_uri={3}&display={4}&immediate={5}&state={6}&scope={7}",
                loginUrl,
                responseType.ToString().ToLower(),
                clientId,
                redirectUrl,
                display.ToString().ToLower(),
                immediate,
                state,
                scope);

            return url;
        }

        public static string FormatRefreshTokenUrl(
            string tokenRefreshUrl,
            string clientId,
            string refreshToken,
            string clientSecret = "")
        {
            if (tokenRefreshUrl == null) throw new ArgumentNullException("tokenRefreshUrl");
            if (clientId == null) throw new ArgumentNullException("clientId");
            if (refreshToken == null) throw new ArgumentNullException("refreshToken");

            var clientSecretQuerystring = "";
            if (!string.IsNullOrEmpty(clientSecret))
            {
                clientSecretQuerystring = string.Format("&client_secret={0}", clientSecret);
            }

            var url =
            string.Format(
                "{0}?grant_type=refresh_token&client_id={1}{2}&refresh_token={3}",
                tokenRefreshUrl,
                clientId,
                clientSecretQuerystring,
                refreshToken);

            return url;
        }
    }
}
