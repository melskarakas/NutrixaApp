using log4net.Repository.Hierarchy;
using ORM.Business;
using ORM.Business.Classes;
using ORM.Models.Models;
using ORM.Models.Models.ViewModels;
using ORM.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ORM.Shared.Base;

namespace ORM.Services.Services
{
    public class NutritionService : INutritionService
    {
        private static bool result = false;
        #region Food İşlemleri
        public bool AddFood(foods food)
        {
            try
            {
                if (food != null)
                {
                    result = new GenericBusiness<foods>().Add(food);
                }
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[AddFood] Food eklenirken bir  hata oluştu.", ex);
                string json = JsonHelper.SerializeObject(food);
                NutrixaLogger.LogInfo($"[AddFood] Hata oluşan food verisi: {json}");
            }
            return result;
        }

        public bool DeleteFood(Guid food_id)
        {
            try
            {
                if (food_id != Guid.Empty)
                {
                    foods food = new GenericBusiness<foods>().GetById(food_id);
                    if (food != null)
                    {
                        food.is_active = false;
                        food.is_deleted = true;
                        food.modified_date = DateTime.Now;
                        result = new GenericBusiness<foods>().Update(food);
                    }
                }
            }
            catch (Exception ex)
            {

                NutrixaLogger.LogError($"[DeleteFood] Food silinirken bir  hata oluştu. Food ID: {food_id}", ex);

            }
            return result;
        }

        public List<foods> GetAll(bool is_active)
        {
            List<foods> foodList = new List<foods>();
            try
            {
                string where = $" AND {nameof(foods.is_active)}='{is_active}'";
                foodList = new GenericBusiness<foods>().GetAllByCustomQuery(where).ToList();
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[GetAll] Food listesi getirilirken bir  hata oluştu.", ex);

            }
            return foodList;
        }



        public List<foods> GetFoodsByCategory(Guid category_id)
        {
            List<foods> foodList = new List<foods>();
            try
            {
                string where = $" AND {nameof(foods.category_id)}='{category_id}'";
                foodList = new GenericBusiness<foods>().GetAllByCustomQuery(where).ToList();
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError($"[GetFoodsByCategory] Food listesi getirilirken bir  hata oluştu. Category ID: {category_id}", ex);

            }
            return foodList;

        }

        public List<vw_foods> GetVwAll(bool is_active)
        {
            List<vw_foods> foodList = new List<vw_foods>();
            try
            {
                string where = $" AND {nameof(foods.is_active)}={is_active}";
                foodList = new GenericBusiness<vw_foods>().GetAllByCustomQuery(where).ToList();
            }
            catch (Exception ex)
            {

                NutrixaLogger.LogError("[GetVwAll] Food listesi getirilirken bir  hata oluştu.", ex);

            }
            return foodList;
        }

        public bool UpdateFood(foods food)
        {
            try
            {
                if (food != null)
                {
                    food.modified_date = DateTime.Now;
                    result = new GenericBusiness<foods>().Update(food);
                }
            }
            catch (Exception ex)
            {

                NutrixaLogger.LogError("[UpdateFood] Food güncellenirken bir  hata oluştu.", ex);
                string json = JsonHelper.SerializeObject(food);
                NutrixaLogger.LogInfo($"[UpdateFood] Hata oluşan food verisi: {json}");

            }
            return result;
        }
        #endregion

        #region Food Category İşlemleri
        public List<food_categories> GetCategories(bool? is_active)
        {
            List<food_categories> categoryList = new List<food_categories>();
            try
            {
                if (is_active.HasValue)
                {
                    string where = $" AND {nameof(food_categories.is_active)}='{is_active}'";
                    categoryList = new GenericBusiness<food_categories>().GetAllByCustomQuery(where).ToList();
                }
                else
                {
                    categoryList = new GenericBusiness<food_categories>().GetAll().ToList();
                }
                return categoryList;

            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[GetCategories] Kategori listesi getirilirken hata oluştu.", ex);
            }
            return categoryList;
        }

        public ReturnType AddCategory(food_categories category)
        {
            try
            {
                if (category == null)
                    return ReturnType.NullRequestData;
                if (!UniqueCategoryName(category.category_name))
                    return ReturnType.NotUnique;
                category.id = Guid.NewGuid();
                bool res= new GenericBusiness<food_categories>().Add(category);
                return res ? ReturnType.Success : ReturnType.Failed;
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[AddCategory] Kategori eklenirken hata oluştu.", ex);
                string json = JsonHelper.SerializeObject(category);
                NutrixaLogger.LogInfo($"[AddCategory] Hata oluşan kategori verisi: {json}");
                return ReturnType.Failed; ;
            }
        }
        private bool UniqueCategoryName(string name)
        {
            bool unique = false;
            try
            {
                string where = $" AND TRIM({nameof(food_categories.category_name)}) ILIKE TRIM('{name}')";
                var existingCategory = new GenericBusiness<food_categories>().GetAllByCustomQuery(where, 1, "").FirstOrDefault();
                unique = existingCategory == null;
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError($"[UniqueCategoryName] Kategori adı kontrol edilirken hata oluştu. Kategori Adı: {name}", ex);
            }
            return unique;
        }
        public ReturnType DeleteCategory(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return ReturnType.NullRequestData;
                food_categories category = new GenericBusiness<food_categories>().GetById(id);
                if (category == null)
                    return ReturnType.NullResponsetData;
                if (GetFoodsByCategory(id).Count > 0)
                    return ReturnType.ConnectData;
                category.is_active = false;
                category.is_deleted = true;
                category.modified_date = DateTime.Now;
                bool res=new GenericBusiness<food_categories>().Update(category);
                return res ? ReturnType.Success : ReturnType.Failed;
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError($"[DeleteCategory] Kategori silinirken hata oluştu. Kategori ID: {id}", ex);
                return ReturnType.Failed;
            }
        }

        public ReturnType UpdateCategory(food_categories category)
        {
            try
            {
                if (category == null)
                    return ReturnType.NullRequestData;
                category.modified_date = DateTime.Now;
                bool res= new GenericBusiness<food_categories>().Update(category);
                return res ? ReturnType.Success : ReturnType.Failed;
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[UpdateCategory] Kategori güncellenirken hata oluştu.", ex);
                string json = JsonHelper.SerializeObject(category);
                NutrixaLogger.LogInfo($"[UpdateCategory] Hata oluşan kategori verisi: {json}");
                return ReturnType.Failed;
            }
        }

        public bool ReturnCategory(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return false;
                food_categories category = new GenericBusiness<food_categories>().GetById(id);
                if (category == null)
                    return false;
                category.is_active = true;
                category.is_deleted = false;
                category.modified_date = DateTime.Now;
                return new GenericBusiness<food_categories>().Update(category);
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError($"[ReturnCategory] Kategori geri getirilirken hata oluştu. Kategori ID: {id}", ex);
                return false;
            }
        }

        public food_categories GetCategoryById(Guid id)
        {
            food_categories category = new food_categories();
            try
            {
                if (id!=Guid.Empty)
                {
                    category=new GenericBusiness<food_categories>().GetById(id);
                }
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError($"[GetCategoryById] Kategori getirilirken hata oluştu. Kategori ID: {id}", ex);
            }
            return category;
        }
        #endregion


    }
}
