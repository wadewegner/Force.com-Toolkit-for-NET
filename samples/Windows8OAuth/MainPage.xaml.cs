using System;
using System.Net;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows8OAuth.Models;
using Salesforce.Common;

namespace Windows8OAuth
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string AuthorizationEndpointUrl = "https://login.salesforce.com/services/oauth2/authorize";
        private const string ConsumerKey = "YOURCONSUMERKEY";
        private const string CallbackUrl = "sfdc://success";
        private Token _token;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _token = await GetAccessToken();

                var message = string.Format("User-Agent Auth Successful:\n\nAccess Token: {0}\n\nRefresh Token: {1}\n\nInstance URL: {2}",
                    _token.AccessToken, _token.RefreshToken, _token.InstanceUrl);

                lblOutput.Text = message;
            }
            catch (Exception ex)
            {
                lblOutput.Text = ex.Message;
            }
        }

        private async void btnRefreshToken_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var auth = new AuthenticationClient();
                await auth.TokenRefreshAsync(ConsumerKey, _token.RefreshToken);

                var message = string.Format("Token Refresh Successful:\n\nAccess Token: {0}\n\nInstance URL: {1}",
                        auth.AccessToken, auth.InstanceUrl);

                lblOutput.Text = message;
            }
            catch (ForceException ex)
            {
                lblOutput.Text = ex.Message;
            }
        }

        private async Task<Token> GetAccessToken()
        {
            var token = new Token();

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

                    token.AccessToken = decoder.GetFirstValueByName("access_token");
                    token.RefreshToken = decoder.GetFirstValueByName("refresh_token");
                    token.InstanceUrl = WebUtility.UrlDecode(decoder.GetFirstValueByName("instance_url"));

                    return token;

                case Windows.Security.Authentication.Web.WebAuthenticationStatus.ErrorHttp:
                    throw new Exception(webAuthenticationResult.ResponseErrorDetail.ToString());

                default:
                    throw new Exception(webAuthenticationResult.ResponseData);
            }
        }


    }
}
