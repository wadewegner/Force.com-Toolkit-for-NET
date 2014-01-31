using System;
using System.Windows.Navigation;

namespace WindowsPhoneOAuth
{
    class AssociationUriMapper : UriMapperBase
    {
        public override Uri MapUri(Uri uri)
        {
            var tempUri = System.Net.HttpUtility.UrlDecode(uri.ToString());

            if (tempUri.Contains("sfdc://success"))
            {
                var querystring = tempUri.Substring(tempUri.IndexOf("#") + 1);
                var split = querystring.Split('&');
                var accessToken = split[0].Split('=')[1];
                var instanceUrl = split[2].Split('=')[1];

                var url = string.Format("/MainPage.xaml?AccessToken={0}&InstanceUrl={1}", accessToken, instanceUrl);

                return new Uri(url, UriKind.Relative);
            }

            return uri;
        }
    }
}
