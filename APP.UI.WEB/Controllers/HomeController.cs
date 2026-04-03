using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APP.UI.WEB.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using ORM.Models;
using ORM.Business;
using ORM.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using APP.UI.WEB.BaseClasses;
using ORM.Models.Models.CustomModels;
using ORM.Models.Models;

namespace APP.UI.WEB.Controllers
{
    public class HomeController : BaseController // : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<SharedResources> _localizer;
        CurrentSession CurrentSession;
        private readonly ISession _session;

        public HomeController(ILogger<HomeController> logger, IStringLocalizer<SharedResources> localizer, IHttpContextAccessor session)
        {
            _logger = logger;
            _localizer = localizer;
            _session = session.HttpContext.Session;
            CurrentSession = new CurrentSession(_session);
        }

        public IActionResult Index(string p = "")
        {
            users user = Operations.GET<users>($"Accounts/GetById?id={CurrentSession.userAuthInfo.id}");

            var days = Operations.POST("Calculates/GetTargetDays", user);
            ViewBag.TargetDay = int.Parse(days);
            return View(user);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

    }
}
