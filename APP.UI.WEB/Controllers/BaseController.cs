using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace APP.UI.WEB.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Do something before the action executes.
            string x = MethodBase.GetCurrentMethod() + " - " + context.HttpContext.Request.Path;

            bool oturumAcik = new BaseClasses.OturumKontrol(context.HttpContext.Session).KullaniciKontrol();
            if (!oturumAcik)
            {
                context.Result = new RedirectResult("/Login/Index");
                //context.Redirect("/Login/Index", true);
                return;
            }
        }
    }
}
