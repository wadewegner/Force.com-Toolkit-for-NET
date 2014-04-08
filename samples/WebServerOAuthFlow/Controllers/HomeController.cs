using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Salesforce.Common;
using Salesforce.Common.Models;

namespace WebServerOAuthFlow.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _authorizationEndpointUrl = ConfigurationSettings.AppSettings["AuthorizationEndpointUrl"];
        private readonly string _consumerKey = ConfigurationSettings.AppSettings["ConsumerKey"];
        private readonly string _callbackUrl = ConfigurationSettings.AppSettings["CallbackUrl"];

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

                ViewBag.Token = token;
                ViewBag.ApiVersion = apiVersion;
                ViewBag.InstanceUrl = instanceUrl;

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

    }
}
