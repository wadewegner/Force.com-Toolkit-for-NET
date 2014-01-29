using System;
using System.Windows.Navigation;

namespace WindowsPhoneOAuth
{
    class AssociationUriMapper : UriMapperBase
    {
        private string tempUri;
        public override Uri MapUri(Uri uri)
        {
            tempUri = System.Net.HttpUtility.UrlDecode(uri.ToString());
            
            if (tempUri.Contains("sfdc://success"))
            {
                var code = tempUri.Substring(tempUri.IndexOf("code=") + 5);
                return new Uri("/MainPage.xaml?Token=" + code, UriKind.Relative);
            }

            return uri;
        }
    }
}
