using ORM.Business.Classes;
using ORM.Models.Models;
using ORM.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ORM.Shared.Base;

namespace ORM.Services.Services
{
    public class CalculateService : ICalculateService
    {
        public decimal CalculateDailyProtein(users user)
        {
            try
            {
                decimal protein = 0.0m;
                if (user != null)
                {
                    if (user.workout_status == WorkoutStatus.AzHareketli)
                    {
                        protein = Math.Round(user.weight * 1.3m, 2);
                    }
                    else if (user.workout_status == WorkoutStatus.OrtaDereceHareketli)
                    {
                        protein = Math.Round(user.weight * 1.5m, 2);
                    }
                    else if (user.workout_status == WorkoutStatus.CokHareketli)
                    {
                        protein = Math.Round(user.weight * 1.8m, 2);
                    }
                    else if (user.workout_status == WorkoutStatus.ProfesyonelSporcu)
                    {
                        protein = Math.Round(user.weight * 2.2m, 2);
                    }
                    else
                    {
                        protein = Math.Round(user.weight * 0.9m, 2);
                    }
                }
                return protein;
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[CalculateDailyProtein] Günlük protein ihtiyacı hesaplanırken bir  hata oluştu.", ex);
                return 0.0m;
            }
        }

        public decimal CalculateBMR(users user)
        {
            decimal bmr= 0.0m;
            try
            {
                if (user != null)
                {

                     bmr = (10 * user.weight) + (6.25m * user.height) - (5 * user.age);
                    if (user.gender == Gender.Kadin)
                    {
                        bmr -= 161;
                    }
                    else
                    {
                        bmr += 5;
                    }
                }
                NutrixaLogger.LogInfo($"[CalculateBMR] BMR hesaplandı BMR : {bmr}");
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[CalculateBMR] BMR hesaplanırken bir  hata oluştu.", ex);
               
            }
            return bmr;
        }
        public decimal CalculateTDEE(users user)
        {
            try
            {
                decimal tdee = 0.0m;
                if (user != null && user.bmr > 0)
                {
                    if (user.workout_status == WorkoutStatus.AzHareketli)
                    {
                        tdee = user.bmr * 1.375m;
                    }
                    else if (user.workout_status == WorkoutStatus.OrtaDereceHareketli)
                    {
                        tdee = user.bmr * 1.55m;
                    }
                    else if (user.workout_status == WorkoutStatus.CokHareketli)
                    {
                        tdee = user.bmr * 1.725m;
                    }
                    else if (user.workout_status == WorkoutStatus.ProfesyonelSporcu)
                    {
                        tdee = user.bmr * 1.9m;
                    }
                    else
                    {
                        tdee = user.bmr * 1.2m;
                    }
                }
                return tdee;
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[CalculateTDEE] Günlük kalori ihtiyacı hesaplanırken bir  hata oluştu.", ex);
                return 0.0m;
            }
        }
        public decimal CalculateBMI(users user)
        {
            try
            {
                decimal bmi = 0.0m;
                if (user != null)
                {
                    bmi = user.weight / ((user.height / 100m) * (user.height / 100m));
                }
                return Math.Round(bmi, 2);
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[CalculateBMI] BMI hesaplanırken bir  hata oluştu.", ex);
                return 0.0m;
            }
        }

        public int CalculateTargetDays(users user)
        {
            try
            {
                int days = 0;
                if (user != null)
                {
                    // 1 kilo = 7.000 kcal
                    int fark = user.weight - user.target_weight;
                    int kalori_acik = 500;
                    int toplam_kalori = fark * 7000;
                    days = toplam_kalori / kalori_acik;
                }
                return days;
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[CalculateTargetDays] Hedefe kalan gün sayısı hesaplanırken bir  hata oluştu.", ex);

                return 0;
            }
        }

        public int CalculateAge(users user)
        {
            int age = 0;
            DateTime today = DateTime.Today;
            try
            {

                if (user != null)
                {
                    age = today.Year - user.date_of_birth.Year;

                    // Doğum günü bu yıl henüz gelmediyse 1 düş
                    if (today < user.date_of_birth.AddYears(age))
                    {
                        age--;
                    }
                }
                return age;
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[CalculateAge] Yaş hesaplanırken bir  hata oluştu.", ex);
                return age;
            }
        }
    }
}
