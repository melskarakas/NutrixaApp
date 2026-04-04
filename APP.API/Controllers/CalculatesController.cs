using APP.API.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ORM.Business.Classes;
using ORM.Models.Models;
using ORM.Services.IServices;
using ORM.Services.Services;
using System;
using System.Threading.Tasks;

namespace APP.API.Controllers
{
    [AuthorizeAttribute]
    [ApiController]
    [Route("[controller]")]
    public class CalculatesController : ControllerBase
    {
        private readonly ICalculateService _calculateService;
        public CalculatesController(ICalculateService calculateService)
        {
            _calculateService = calculateService;
        }

        [HttpPost]
        [Route("GetTargetDays")]
        public ObjectResult GetTargetDays(users user)
        {
            try
            {
                return Ok(_calculateService.CalculateTargetDays(user));
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[GetTargetDays] Hedef gün sayısı hesaplanırken bir hata oluştu.", ex);
                string userJson = JsonHelper.SerializeObject(user);
                NutrixaLogger.LogInfo($"[GetTargetDays] Hata oluşan kullanıcı verisi: {userJson}");
                return Problem(detail: "Genel Hata: " + ex.Message, statusCode: 500);
            }
        }
    }
}
