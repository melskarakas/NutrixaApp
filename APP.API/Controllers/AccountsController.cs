using APP.API.Auth;
using ORM.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;
using ORM.Models.Models;
using ORM.Models.Models.CustomModels;

namespace APP.API.Controllers
{
    [AuthorizeAttribute]
    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountsController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICalculateService _calculateService;
        public AccountsController(IUserService userService,ICalculateService calculateService)
        {
            _userService = userService;
            _calculateService = calculateService;
        }

        [HttpGet]
        public async Task<ObjectResult> GetById(Guid id)
        {
            try
            {
                var user = await _userService.GetUser(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return Problem(detail: "Genel Hata: " + ex.Message, statusCode: 500);
            }
        }
        [HttpGet]
        public async Task<ObjectResult> GetAll()
        {
            try
            {
                var list = await _userService.GetAll();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Problem(detail: "Genel Hata: " + ex.Message, statusCode: 500);
            }
        }
        [HttpPut]
        public async Task<ObjectResult> Update([FromBody] UserUpdateRequest updateRequest)
        {
            try
            {
                if (!updateRequest.IsCalculationUpdate)
                {
                    updateRequest.UpdatedItem.bmr = _calculateService.CalculateBMR(updateRequest.UpdatedItem);
                    updateRequest.UpdatedItem.tdee = _calculateService.CalculateTDEE(updateRequest.UpdatedItem);
                    updateRequest.UpdatedItem.bmi = _calculateService.CalculateBMI(updateRequest.UpdatedItem);
                    updateRequest.UpdatedItem.dpr = _calculateService.CalculateDailyProtein(updateRequest.UpdatedItem);
                    updateRequest.UpdatedItem.age = _calculateService.CalculateAge(updateRequest.UpdatedItem);
                }
                var res = await _userService.Update(updateRequest.UpdatedItem);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Problem(detail: "Genel Hata: " + ex.Message, statusCode: 500);
            }
        }
        [HttpPost]
        public async Task<ObjectResult> Add([FromBody] users user)
        {
            try
            {
                user.bmr = _calculateService.CalculateBMR(user);
                user.tdee = _calculateService.CalculateTDEE(user);
                user.bmi = _calculateService.CalculateBMI(user);
                user.dpr = _calculateService.CalculateDailyProtein(user);
                user.age = _calculateService.CalculateAge(user);
                var added = await _userService.AddUser(user);
                return Ok(added);
            }
            catch (Exception ex)
            {
                return Problem(detail: "Genel Hata: " + ex.Message, statusCode: 500);
            }
        }
    }
}
