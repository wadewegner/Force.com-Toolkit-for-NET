using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Salesforce.Common;
using Salesforce.Force;
using WindowsPhoneOAuth.Models;

namespace WindowsPhoneOAuth
{
    public partial class MainPage : PhoneApplicationPage
    {
        private const string AuthorizationEndpointUrl = "https://login.salesforce.com/services/oauth2/authorize";
        private const string ConsumerKey = "YOURCONSUMERKEY";
        private const string ConsumerSecret = "YOURCONSUMERSECRET";
        private const string CallbackUrl = "sfdc://success";
        
        public string AccessToken { get; set; }
        
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("Token"))
            {
                AccessToken = NavigationContext.QueryString["Token"];
            }
            base.OnNavigatedTo(e);
        }

        async private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(AccessToken))
            {
                OrganizationsList.Visibility = Visibility.Collapsed;
                AuthBrowser.Visibility = Visibility.Visible;

                var url =
                    Common.FormatAuthUrl(
                        AuthorizationEndpointUrl,
                        ResponseTypes.Code,
                        ConsumerKey,
                        CallbackUrl,
                        DisplayTypes.Touch);

                AuthBrowser.Navigate(new Uri(url));

                return;
            }

            var auth = new AuthenticationClient();
            await auth.WebServer(ConsumerKey, ConsumerSecret, CallbackUrl, AccessToken);

            var client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);
            var accounts = await client.Query<Account>("SELECT id, name, description FROM Account");

            OrganizationsList.Visibility = Visibility.Visible;
            AuthBrowser.Visibility = Visibility.Collapsed;

            OrganizationsList.ItemsSource = accounts;
        }

    }
}