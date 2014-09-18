using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Salesforce.Common;
using Salesforce.Common.Models;

namespace $rootnamespace$.Controllers
{
    public class LoginController : Controller
    {
        private readonly string _authorizationEndpointUrl = ConfigurationManager.AppSettings["AuthorizationEndpointUrl"];
        private readonly string _consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
        private readonly string _consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
        private readonly string _callbackUrl = ConfigurationManager.AppSettings["CallbackUrl"];

        public ActionResult Login()
        {
            var url =
                Common.FormatAuthUrl(
                    _authorizationEndpointUrl,
                    ResponseTypes.Code,
                    _consumerKey,
                    HttpUtility.UrlEncode(_callbackUrl));

            return Redirect(url);
        }

        public async Task<ActionResult> Callback(string display, string code)
        {
            var auth = new AuthenticationClient();
            await auth.WebServerAsync(_consumerKey, _consumerSecret, _callbackUrl, code, "");

            Session["ApiVersion"] = auth.ApiVersion;
            Session["AccessToken"] = auth.AccessToken;
            Session["InstanceUrl"] = auth.InstanceUrl;

            return RedirectToAction("Index", "Home");
        }
    }
}