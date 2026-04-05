using ORM.Models.Models;
using ORM.Models.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ORM.Shared.Base;

namespace ORM.Services.IServices
{
    public interface INutritionService
    {
        List<foods> GetFoodsByCategory(Guid category_id);
        List<food_categories> GetCategories(bool? is_active);
        ReturnType AddCategory(food_categories category);
        ReturnType DeleteCategory(Guid id);
        bool ReturnCategory(Guid id);
        ReturnType UpdateCategory(food_categories category);
        food_categories GetCategoryById(Guid id);
        List<foods> GetAll(bool is_active);
        List<vw_foods> GetVwAll(bool is_active);
        bool AddFood(foods food);
        bool UpdateFood(foods food);
        bool DeleteFood(Guid food_id);
    }
}
