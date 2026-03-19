using APP.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using ORM.Services.IServices;
using ORM.Services.Models;

namespace APP.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthenticateController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthenticateController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        public async Task<IActionResult> GetToken([FromBody] APP.API.Models.AuthenticateRequest model)
        {
            try
            {
                var result = await _userService.AuthenticateByEmail(model.Email, model.Password, model.IsPasswordMd5);
                if (result == null || string.IsNullOrEmpty(result.Token))
                {
                    return BadRequest(new { message = "Email veya parola hatalı." });
                }

                return Ok(new { token = result.Token, is_registration = result.IsRegistration });
            }
            catch (Exception ex)
            {
                return Problem(detail: "Genel Hata: " + ex.Message, statusCode: 500);
            }
        }
    }
}
