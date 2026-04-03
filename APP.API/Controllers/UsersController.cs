using Microsoft.AspNetCore.Mvc;
using ORM.Business;
using ORM.Models;
using System;
using System.Threading.Tasks;
using ORM.Services.IServices;
using APP.API.Models;
using ORM.Services.Models;
using APP.API.Auth;
using ORM.Models.Models;
using ORM.Services.Services;

namespace APP.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICalculateService _calculateService;

        public UsersController(IUserService userService,ICalculateService calculateService)
        {
            _userService = userService;
            _calculateService = calculateService;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] APP.API.Models.AuthenticateRequest model)
        {
            var result = await _userService.AuthenticateByEmail(model.Email, model.Password, model.IsPasswordMd5);

            if (result == null || string.IsNullOrEmpty(result.Token))
                return BadRequest(new { message = "Kullanıcı adı veya parola yanlış." });

            return Ok(new { token = result.Token, is_registration = result.IsRegistration });
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Mail) || string.IsNullOrWhiteSpace(model.Password))
                    return BadRequest(new { message = "Mail ve parola gerekli." });

                // check existing email/username and return specific message
                bool emailExists = await _userService.ExistsByEmail(model.Mail);
                if (emailExists)
                {
                    if (emailExists)
                        return Conflict(new { message = "E-posta ve kullanıcı adı zaten kayıtlı." });
                    if (emailExists)
                        return Conflict(new { message = "E-posta zaten kayıtlı." });
                    return Conflict(new { message = "Kullanıcı adı zaten kayıtlı." });
                }

                // map DTO to users model
                var user = new users
                {
                    id = Guid.NewGuid(),
                    email = model.Mail,
                    password = model.Password,
                    first_name = model.FirstName, // prefer provided username
                    last_name = model.LastName,
                    user_type = 1,
                    last_login_time = DateTime.MinValue,
                    created_date = DateTime.Now,
                    modified_date = DateTime.Now,
                    is_active = true,
                    is_deleted = false,
                };

                // additional optional fields can be stored in extended table or ignored for now
                user.bmr = _calculateService.CalculateBMR(user);
                user.tdee = _calculateService.CalculateTDEE(user);
                user.bmi = _calculateService.CalculateBMI(user);
                user.dpr = _calculateService.CalculateDailyProtein(user);
                var added = await _userService.AddUser(user);
                if (added == null)
                    return Conflict(new { message = "Kayıt oluşturulamadı. Lütfen bilgileri kontrol edin." });

                return Ok(added);
            }
            catch (Exception ex)
            {
                return Problem(detail: "Genel Hata: " + ex.Message, statusCode: 500);
            }
        }

    }
}
