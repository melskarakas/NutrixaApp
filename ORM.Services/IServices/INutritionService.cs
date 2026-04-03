using ORM.Models.Models;
using ORM.Models.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.Services.IServices
{
    public interface INutritionService
    {
        List<foods> GetFoodsByCategory(Guid category_id);
        List<food_categories> GetCategories(bool is_active);
        List<foods> GetAll(bool is_active);
        List<vw_foods> GetVwAll(bool is_active);
        bool AddFood(foods food);
        bool UpdateFood(foods food);
        bool DeleteFood(Guid food_id);
    }
}
