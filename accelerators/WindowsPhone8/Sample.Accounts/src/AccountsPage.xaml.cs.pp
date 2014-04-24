using System.Windows;
using Microsoft.Phone.Controls;
using $rootnamespace$.Models;
using Salesforce.Force;

namespace $rootnamespace$.Pages
{
    public partial class AccountsPage : PhoneApplicationPage
    {
        public AccountsPage()
        {
            InitializeComponent();
        }

        private async void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            var app = (Application.Current as App);

            if (app != null)
            {
                if ((!string.IsNullOrEmpty(app.InstanceUrl)) & (!string.IsNullOrEmpty(app.AccessToken)))
                {
                    var client = new ForceClient(app.InstanceUrl, app.AccessToken, "v29.0");
                    var accounts = await client.QueryAsync<Account>("SELECT id, name, description FROM Account");

                    AccountsList.ItemsSource = accounts.records;
                }
            }
        }
    }
}