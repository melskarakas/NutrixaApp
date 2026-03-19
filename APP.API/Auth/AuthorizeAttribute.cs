using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using ORM.Business;
using System.Linq;
using ORM.Models.Models;

namespace APP.API.Auth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (users?)context.HttpContext.Items["User"];
            string token = context.HttpContext.Request.Headers["Token"];
            if (user == null)
            {
                if (!String.IsNullOrEmpty(token))
                {
                    if (!tokenControl(token))
                    {
                        //token geçerli değilse
                        context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                    }

                }             
            }
        }
        public bool tokenControl(string token)
        {
            string w = $" AND {nameof(authtoken.token)}='{token}'";
            authtoken _token = new GenericBusiness<authtoken>().GetAllByCustomQuery(w).FirstOrDefault();
            if (_token != null)
            {
                if (_token.expire_date > DateTime.Now)
                {
                    return true;
                }
                else
                {
                    //token süresi dolmuş
                    return false;
                }
            }
            else
            {
                //token sistemde yok
                return false;
            }
        }
    }
}
