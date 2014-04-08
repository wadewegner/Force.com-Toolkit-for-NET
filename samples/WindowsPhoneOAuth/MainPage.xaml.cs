using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Salesforce.Common;
using Salesforce.Common.Models;
using Salesforce.Force;
using WindowsPhoneOAuth.Models;

namespace WindowsPhoneOAuth
{
    public partial class MainPage : PhoneApplicationPage
    {
        private const string AuthorizationEndpointUrl = "https://login.salesforce.com/services/oauth2/authorize";
        private const string ConsumerKey = "YOURCONSUMERKEY";
        private const string CallbackUrl = "sfdc://success";
        private const string ApiVersion = "v29.0";

        private string _accessToken;
        private string _instanceUrl;
        
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("AccessToken"))
            {
                _accessToken = NavigationContext.QueryString["AccessToken"];
            }
            if (NavigationContext.QueryString.ContainsKey("InstanceUrl"))
            {
                _instanceUrl = NavigationContext.QueryString["InstanceUrl"];
            }
            base.OnNavigatedTo(e);
        }

        async private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                OrganizationsList.Visibility = Visibility.Collapsed;
                AuthBrowser.Visibility = Visibility.Visible;

                var url =
                    Common.FormatAuthUrl(
                        AuthorizationEndpointUrl,
                        ResponseTypes.Token,
                        ConsumerKey,
                        CallbackUrl,
                        DisplayTypes.Touch);

                AuthBrowser.Navigate(new Uri(url));

                return;
            }

            var client = new ForceClient(_instanceUrl, _accessToken, ApiVersion);
            var accounts = await client.QueryAsync<Account>("SELECT id, name, description FROM Account");

            OrganizationsList.ItemsSource = accounts.records;

            OrganizationsList.Visibility = Visibility.Visible;
            AuthBrowser.Visibility = Visibility.Collapsed;
        }
    }
}