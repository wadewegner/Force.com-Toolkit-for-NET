﻿using System;
using Salesforce.Common.Models;

namespace Salesforce.Common
{
    public static class Common
    {
        public static string FormatUrl(string resourceName, string instanceUrl, string apiVersion, string parameters=null)
        {
            if (string.IsNullOrEmpty(resourceName)) throw new ArgumentNullException("resourceName");
            if (string.IsNullOrEmpty(instanceUrl)) throw new ArgumentNullException("instanceUrl");
            if (string.IsNullOrEmpty(apiVersion)) throw new ArgumentNullException("apiVersion");

            if (resourceName.StartsWith("/services/data", StringComparison.CurrentCultureIgnoreCase))
            {
                string formattedUrl = string.Format("{0}{1}", instanceUrl, resourceName);
                if (parameters != null)
                {
                    string.Format("{0}{1}", formattedUrl, parameters);
                }
                return formattedUrl;
            }
            
            string formatted_Url = string.Format("{0}/services/data/{1}/{2}", instanceUrl, apiVersion, resourceName);
            if (parameters != null)
            {
                formatted_Url = string.Format("{0}{1}", formatted_Url, parameters);
            }
            return formatted_Url;
        }

		/// <summary>
        /// Format url using /services/apexrest for calling customer REST APIs
        /// </summary>
        /// <param name="customAPI">The name of the custom REST API</param>
        /// <param name="parameters">Pre-formatted parameters like this: ?name1=value1&name2=value2&soon=soforth</param>
        /// <param name="instanceUrl">Instance url returned from auth</param>
        /// <returns>String: The formatted Url</returns>
        public static string FormatCustomUrl(string customAPI, string parameters, string instanceUrl)
        {
            if (string.IsNullOrEmpty(customAPI)) throw new ArgumentNullException("customAPI");
            if (string.IsNullOrEmpty(instanceUrl)) throw new ArgumentNullException("instanceUrl");

            // Some urls may not require params, so, handle the case where there are none by creating the formatted
            // url and then check to see if params need to be added.
            string formattedUrl = string.Format("{0}/services/apexrest/{1}", instanceUrl, customAPI);
            if (string.IsNullOrEmpty(parameters))
            {
                formattedUrl = string.Format("{0}{1}", formattedUrl, parameters);
            }
            return formattedUrl;
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
