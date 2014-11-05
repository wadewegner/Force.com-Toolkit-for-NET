using System;
using System.Windows;
using System.Windows.Navigation;
using WindowsPhoneOAuth8._0.Models;
using Microsoft.Phone.Controls;
using Salesforce.Common;
using Salesforce.Common.Models;
using Salesforce.Force;

namespace WindowsPhoneOAuth8._0
{
    public partial class MainPage : PhoneApplicationPage
    {
        private const string AuthorizationEndpointUrl = "https://login.salesforce.com/services/oauth2/authorize";
        private const string ConsumerKey = "3MVG9JZ_r.QzrS7izXVWrETc3v6O4sn7UWEn8DzJh.fmviPqTUyglbO18GFm4nb3psLLh1BTWMMtcOKVVnfc4";
        private const string CallbackUrl = "sfdc://success";
        private const string ApiVersion = "v32.0";

        private string _accessToken;
        private string _instanceUrl;

        // Constructor
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

        private async void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
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


        //private async Task PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (string.IsNullOrEmpty(_accessToken))
        //    {
        //        OrganizationsList.Visibility = Visibility.Collapsed;
        //        AuthBrowser.Visibility = Visibility.Visible;

        //        var url =
        //            Common.FormatAuthUrl(
        //                AuthorizationEndpointUrl,
        //                ResponseTypes.Token,
        //                ConsumerKey,
        //                CallbackUrl,
        //                DisplayTypes.Touch);

        //        AuthBrowser.Navigate(new Uri(url));
        //    }


        //    var client = new ForceClient(_instanceUrl, _accessToken, ApiVersion);
        //    var accounts = await client.QueryAsync<Account>("SELECT id, name, description FROM Account");

        //    OrganizationsList.ItemsSource = accounts.records;

        //    OrganizationsList.Visibility = Visibility.Visible;
        //    AuthBrowser.Visibility = Visibility.Collapsed;
        //}

        
    }
}