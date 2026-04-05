using APP.API.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ORM.Business.Classes;
using ORM.Models.Models;
using ORM.Services.IServices;
using System;

namespace APP.API.Controllers
{
    [AuthorizeAttribute]
    [ApiController]
    [Route("[controller]")]
    public class NutritionsController : ControllerBase
    {
        private readonly INutritionService _nutritionService;
        public NutritionsController(INutritionService nutritionService)
        {
            _nutritionService = nutritionService;
        }

        #region Food Category
        [HttpGet]
        [Route("GetFoodCategories")]
        public ObjectResult GetFoodCategories(bool? is_active)
        {
            try
            {
                return Ok(_nutritionService.GetCategories(is_active));
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[GetFoodCategories] Gıda kategorileri alınırken bir hata oluştu.", ex);
                return Problem(detail: "Genel Hata: " + ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        [Route("GetFoodsByCategory")]
        public ObjectResult GetFoodsByCategory(Guid category_id)
        {
            try
            {
                return Ok(_nutritionService.GetFoodsByCategory(category_id));
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError($"[GetFoodsByCategory] Kategoriye göre gıdalar alınırken bir hata oluştu. Kategori ID: {category_id}", ex);
                return Problem(detail: "Genel Hata: " + ex.Message, statusCode: 500);
            }
        }
        [HttpPost]
        [Route("AddFoodCategory")]
        public ObjectResult AddFoodCategory(food_categories category)
        {
            try
            {
                return Ok(_nutritionService.AddCategory(category));
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[AddFoodCategory] Gıda kategorisi eklenirken bir hata oluştu.", ex);
                string json = JsonHelper.SerializeObject(category);
                NutrixaLogger.LogInfo($"[AddFoodCategory] Hata oluşan kategori verisi: {json}");
                return Problem(detail: "Genel Hata: " + ex.Message, statusCode: 500);
            }
        }
        [HttpPut]
        [Route("UpdateFoodCategory")]
        public ObjectResult UpdateFoodCategory(food_categories category)
        {
            try
            {
                return Ok(_nutritionService.UpdateCategory(category));
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[UpdateFoodCategory] Gıda kategorisi güncellenirken bir hata oluştu.", ex);
                string json = JsonHelper.SerializeObject(category);
                NutrixaLogger.LogInfo($"[UpdateFoodCategory] Hata oluşan kategori verisi: {json}");
                return Problem(detail: "Genel Hata: " + ex.Message, statusCode: 500);
            }
        }
        [HttpPut]
        [Route("DeleteFoodCategory")]
        public ObjectResult DeleteFoodCategory(food_categories entity)
        {
            try
            {
                return Ok(_nutritionService.DeleteCategory(entity.id));
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError($"[DeleteFoodCategory] Gıda kategorisi silinirken bir hata oluştu. Kategori ID: {entity.id}", ex);
                return Problem(detail: "Genel Hata: " + ex.Message, statusCode: 500);
            }
        }
        [HttpPut]
        [Route("ReturnFoodCategory")]
        public ObjectResult ReturnFoodCategory(food_categories entity)
        {
            try
            {
                return Ok(_nutritionService.ReturnCategory(entity.id));
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError($"[ReturnFoodCategory] Gıda kategorisi geri getirilirken bir hata oluştu. Kategori ID: {entity.id}", ex);
                return Problem(detail: "Genel Hata: " + ex.Message, statusCode: 500);
            }
        }
        [HttpGet]
        [Route("GetCategoryById")]
        public ObjectResult GetCategoryById(Guid id)
        {
            try
            {
                return Ok(_nutritionService.GetCategoryById(id));
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError($"[GetCategoryById] Kategoriye göre gıdalar alınırken bir hata oluştu. Kategori ID: {id}", ex);
                return Problem(detail: "Genel Hata: " + ex.Message, statusCode: 500);
            }
        }
        #endregion
    }
}
