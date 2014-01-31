using System;
using System.Net;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Salesforce.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
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

        public MainPage()
        {
            this.InitializeComponent();
        }

        async private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var startUrl = Common.FormatAuthUrl(AuthorizationEndpointUrl, ResponseTypes.Token, ConsumerKey, WebUtility.UrlEncode(CallbackUrl), DisplayTypes.Popup);
            var startUri = new Uri(startUrl);
            var endUri = new Uri(CallbackUrl);
            string result;

            try
            {
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

                        var accessToken = decoder.GetFirstValueByName("access_token");
                        var refreshToken = decoder.GetFirstValueByName("refresh_token");
                        var instanceUrl = WebUtility.UrlDecode(decoder.GetFirstValueByName("instance_url"));

                        result = string.Format("Access Token: {0}\n\nRefresh Token: {1}\n\nInstance URL: {2}", accessToken,
                            refreshToken, instanceUrl);

                        break;
                    case Windows.Security.Authentication.Web.WebAuthenticationStatus.ErrorHttp:

                        result = webAuthenticationResult.ResponseErrorDetail.ToString();
                        break;

                    default:

                        result = webAuthenticationResult.ResponseData;
                        break;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            lblOutput.Text = result;
        }
    }
}
