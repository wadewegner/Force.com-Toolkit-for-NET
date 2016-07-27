using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Salesforce.Common;
using Salesforce.Common.Models;

namespace WebServerOAuthFlow.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _authorizationEndpointUrl = ConfigurationManager.AppSettings["AuthorizationEndpointUrl"];
        private readonly string _consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
        private readonly string _consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
        private readonly string _callbackUrl = ConfigurationManager.AppSettings["CallbackUrl"];

        public HomeController()
        {
            ViewBag.LoggedIn = false;
        }

        public ActionResult Index()
        {
            if (Request.QueryString.HasKeys())
            {
                var token = Request.QueryString["token"];
                var apiVersion = Request.QueryString["api"];
                var instanceUrl = Request.QueryString["instance_url"];
                var refreshToken = Request.QueryString["refresh_token"];

                ViewBag.Token = token;
                ViewBag.ApiVersion = apiVersion;
                ViewBag.InstanceUrl = instanceUrl;
                ViewBag.RefreshToken = refreshToken;

                ViewBag.LoggedIn = true;
            }

            return View();
        }

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

        public async Task<ActionResult> GetRefreshToken(string refreshToken)
        {
            var auth = new AuthenticationClient();
            await auth.TokenRefreshAsync(_consumerKey, refreshToken, _consumerSecret);

            ViewBag.Token = auth.AccessToken;
            ViewBag.ApiVersion = auth.ApiVersion;
            ViewBag.InstanceUrl = auth.InstanceUrl;
            ViewBag.RefreshToken = auth.RefreshToken;
            
            ViewBag.LoggedIn = true;

            return View("Index");
        }
    }
}
