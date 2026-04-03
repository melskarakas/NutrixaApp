using APP.UI.WEB.BaseClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ORM.Models.Models;
using ORM.Models.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace APP.UI.WEB.Controllers
{
    public class AccountController : Controller
    {
        CurrentSession CurrentSession;
        private readonly ISession _session;
        public AccountController(IHttpContextAccessor session)
        {
            _session = session.HttpContext.Session;
            CurrentSession = new CurrentSession(_session);
        }
        public IActionResult Index()
        {
            users user = Operations.GET<users>($"Accounts/GetById?id={CurrentSession.userAuthInfo.id}");
            return View(user);
        }
        public ActionResult Save(Guid id, string first_name, string last_name, string email, string phone, string height, string weight, string date_of_birth, string target_weight, string workout_status, string gender, string age, string bmi, string bmr, string tdee, string dpr, string current_pwd, string new_pwd, bool is_calculation_update)
        {
            users user = Operations.GET<users>($"Accounts/GetById?id={id}");
            if (!String.IsNullOrEmpty(current_pwd) && !String.IsNullOrEmpty(new_pwd))
            {
                string currentPwdMd5 = ORM.Shared.Base.GenerateDoubleMD5Password(current_pwd);
                if (user.password == currentPwdMd5)
                {
                    user.password = !String.IsNullOrEmpty(new_pwd) ? ORM.Shared.Base.GenerateDoubleMD5Password(new_pwd) : user.password;
                }
                else
                {
                    return Json(new { success = false, message = "Mevcut şifre hatalı" });
                }
            }
            user.first_name = first_name;
            user.last_name = last_name;
            user.email = email;
            user.phone_number = phone;
            user.height = !String.IsNullOrEmpty(height) ? int.Parse(height) : user.height;
            user.weight = !String.IsNullOrEmpty(weight) ? int.Parse(weight) : user.weight;
            user.date_of_birth = !String.IsNullOrEmpty(date_of_birth) ? DateTime.Parse(date_of_birth) : user.date_of_birth;
            user.target_weight = !String.IsNullOrEmpty(target_weight) ? int.Parse(target_weight) : user.target_weight;
            if (Enum.TryParse<ORM.Shared.Base.WorkoutStatus>(workout_status, out var result))
            {
                user.workout_status = result;
            }
            if (Enum.TryParse<ORM.Shared.Base.Gender>(gender, out var genderResult))
            {
                user.gender = genderResult;
            }
            user.age = !String.IsNullOrEmpty(age) ? int.Parse(age) : user.age;
            user.bmi = !String.IsNullOrEmpty(bmi) ? decimal.Parse(bmi) : user.bmi;
            user.bmr = !String.IsNullOrEmpty(bmr) ? decimal.Parse(bmr) : user.bmr;
            user.tdee = !String.IsNullOrEmpty(tdee) ? decimal.Parse(tdee) : user.tdee;
            user.dpr = !String.IsNullOrEmpty(dpr) ? decimal.Parse(dpr, CultureInfo.InvariantCulture) : user.dpr;
            UserUpdateRequest updateRequest = new UserUpdateRequest
            {
                UpdatedItem = user,
                IsCalculationUpdate = is_calculation_update
            };
            var res = Operations.PUT("Accounts/Update", updateRequest);
            if (res)
            {
                return Json(new { success = true, message = "İşlem başarılı" });
            }
            else
            {
                return Json(new { success = false, message = "İşlem başarısız" });
            }
        }
        public ActionResult UserDefinition()
        {
            return View();
        }

        public ActionResult GetUserTable()
        {
            List<users> userList = Operations.GET<List<users>>("Accounts/GetAll");
            return PartialView("~/Views/Account/PartialViews/UserTable.cshtml", userList);
        }
        public ActionResult UserModal(Guid userId)
        {
            users user = new users();
            if (userId != Guid.Empty)
            {
                user = Operations.GET<users>($"Accounts/GetById?id={userId}");
            }
            return PartialView("~/Views/Account/PartialViews/UserModal.cshtml", user);
        }
        public ActionResult SaveUser(users user, string new_pwd)
        {
            string res = "";
            if (user.id != Guid.Empty)
            {
                user.password = !String.IsNullOrEmpty(new_pwd) ? ORM.Shared.Base.GenerateDoubleMD5Password(new_pwd) : user.password;
                UserUpdateRequest updateRequest = new UserUpdateRequest
                {
                    UpdatedItem = user,
                    IsCalculationUpdate = false
                };
                bool result = Operations.PUT("Accounts/Update", updateRequest);
                res = result.ToString();
            }
            else
            {
                user.id = Guid.NewGuid();
                user.password = !String.IsNullOrEmpty(new_pwd) ? new_pwd : Guid.NewGuid().ToString().Split('-')[0];
                res = Operations.POST("Accounts/Add", user);
            }

            return Json(res);
        }
        public ActionResult DeleteUser(Guid userId)
        {
            users user = Operations.GET<users>($"Accounts/GetById?id={userId}");
            user.is_active = false;
            user.is_deleted = true;
            var res = Operations.PUT("Accounts/Update", user);
            return Json(res);
        }

        public ActionResult SwitchUser(Guid userId, bool status)
        {
            users user = Operations.GET<users>($"Accounts/GetById?id={userId}");
            user.is_active = !status;
            user.is_deleted = !user.is_deleted;
            var res = Operations.PUT("Accounts/Update", user);
            return Json(res);
        }
    }
}
