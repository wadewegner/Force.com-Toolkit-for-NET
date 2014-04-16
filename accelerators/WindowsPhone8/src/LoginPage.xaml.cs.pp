using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Windows.Foundation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Salesforce.Common;
using Salesforce.Common.Models;

namespace $rootnamespace$.Pages
{
    public partial class LoginPage : PhoneApplicationPage
    {
        private const string AuthorizationEndpointUrl = "https://login.salesforce.com/services/oauth2/authorize";
        private const string ConsumerKey = "YOURCONSUMERKEY";
        private const string CallbackUrl = "sfdc://success";
        private const string ApiVersion = "v29.0";

        public LoginPage()
        {
            InitializeComponent();
            WebBrowser.IsScriptEnabled = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var uri =
                    Common.FormatAuthUrl(
                        AuthorizationEndpointUrl,
                        ResponseTypes.Token,
                        ConsumerKey,
                        CallbackUrl,
                        DisplayTypes.Touch);

            WebBrowser.Navigate(new Uri(uri, UriKind.Absolute));
        }

        private void WebBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            var app = (Application.Current as App);

            var returnUri = System.Net.HttpUtility.UrlDecode(e.Uri.ToString());

            if (returnUri.StartsWith("sfdc://success"))
            {
                e.Cancel = true;

                var querystring = returnUri.Substring(returnUri.IndexOf("#") + 1);
                var split = querystring.Split('&');

                if (app != null)
                {
                    app.AccessToken = split[0].Split('=')[1];
                    app.RefreshToken = split[1].Split('=')[1];
                    app.InstanceUrl = split[2].Split('=')[1];
                }

                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }
    }
}