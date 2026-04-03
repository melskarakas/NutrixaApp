using ORM.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.Services.IServices
{
    public interface ICalculateService
    {
        decimal CalculateDailyProtein(users user);
        decimal CalculateBMR(users user);
        decimal CalculateBMI(users user);
        decimal CalculateTDEE(users user);
        int CalculateTargetDays(users user);
        int CalculateAge(users user);
    }
}
