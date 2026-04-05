using APP.UI.WEB.BaseClasses;
using Microsoft.AspNetCore.Mvc;
using ORM.Models.Models;
using ORM.Shared;
using System;
using System.Collections.Generic;
using static ORM.Shared.Base;

namespace APP.UI.WEB.Controllers
{
    public class NutritionController : Controller
    {
        public IActionResult CategoryDefinition()
        {
            return View();
        }
        public ActionResult GetCategoryTable(bool? is_active)
        {
            List<food_categories> categories = Operations.GET<List<food_categories>>($"Nutritions/GetFoodCategories?is_active={is_active}");
            return PartialView("~/Views/Nutrition/PartialViews/CategoryTable.cshtml", categories);
        }

        public ActionResult DeleteCategory(Guid categoryId)
        {
            food_categories category = new food_categories { id = categoryId };
            var res = Operations.PUT("Nutritions/DeleteFoodCategory", category);
            string result = EnumExtensions.GetDisplayName<ReturnType>(res);
            if (int.Parse(res) == (int)ReturnType.Success)
            {
                return Json(new { success = true, message = result });

            }
            else
            {
                return Json(new { success = false, message = result });

            }
        }
        public ActionResult ReturnCategory(Guid categoryId)
        {
            food_categories category = new food_categories { id = categoryId };
            var result = Operations.PUT("Nutritions/ReturnFoodCategory", category);
            return Json(result);
        }
        public ActionResult CategoryModal(Guid categoryId)
        {
            food_categories category = new food_categories();
            if (categoryId != Guid.Empty)
            {
                category = Operations.GET<food_categories>($"Nutritions/GetCategoryById?id={categoryId}");
            }
            return PartialView("~/Views/Nutrition/PartialViews/CategoryModal.cshtml", category);
        }
        public ActionResult SaveCategory(food_categories category)
        {
            string result;
            string res = string.Empty;
            bool status = false;
            if (category.id == Guid.Empty)
            {
                category.modified_date = DateTime.Now;
                result = Operations.POST("Nutritions/AddFoodCategory", category);
                res = EnumExtensions.GetDisplayName<ReturnType>(result);
                status = int.Parse(result) == (int)ReturnType.Success;
            }
            else
            {
                result = Operations.PUT("Nutritions/UpdateFoodCategory", category).ToString();
                res = EnumExtensions.GetDisplayName<ReturnType>(result);
                status = int.Parse(result) == (int)ReturnType.Success;
            }
            return Json(new { success = status, message = res });
        }
    }
}