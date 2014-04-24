using System;
using System.Net;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Salesforce.Common;
using Salesforce.Common.Models;

namespace $rootnamespace$
{
    public sealed partial class MainPage : Page
    {
        private const string AuthorizationEndpointUrl = "https://login.salesforce.com/services/oauth2/authorize";
        private const string ConsumerKey = "YOURCONSUMERKEY";
        private const string CallbackUrl = "sfdc://success";

        private async Task GetAccessToken()
        {
            var app = (Application.Current as App);
            
            var startUrl = Common.FormatAuthUrl(AuthorizationEndpointUrl, ResponseTypes.Token, ConsumerKey,
                WebUtility.UrlEncode(CallbackUrl), DisplayTypes.Popup);
            var startUri = new Uri(startUrl);
            var endUri = new Uri(CallbackUrl);

            var webAuthenticationResult =
                await Windows.Security.Authentication.Web.WebAuthenticationBroker.AuthenticateAsync(
                    Windows.Security.Authentication.Web.WebAuthenticationOptions.None,
                    startUri,
                    endUri);

            switch (webAuthenticationResult.ResponseStatus)
            {
                case Windows.Security.Authentication.Web.WebAuthenticationStatus.Success:
                    var responseData = webAuthenticationResult.ResponseData;
                    var responseUri = new Uri(responseData);
                    var decoder = new WwwFormUrlDecoder(responseUri.Fragment.Replace("#", "?"));

                    app.AccessToken = decoder.GetFirstValueByName("access_token");
                    app.RefreshToken = decoder.GetFirstValueByName("refresh_token");
                    app.InstanceUrl = WebUtility.UrlDecode(decoder.GetFirstValueByName("instance_url"));

                    return;

                case Windows.Security.Authentication.Web.WebAuthenticationStatus.ErrorHttp:
                    throw new Exception(webAuthenticationResult.ResponseErrorDetail.ToString());

                default:
                    throw new Exception(webAuthenticationResult.ResponseData);
            }
        }


    }
}
