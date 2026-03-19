using Microsoft.AspNetCore.Mvc;
using ORM.Business;
using ORM.Models;
using System;
using System.Threading.Tasks;
using ORM.Services.IServices;
using APP.API.Models;
using ORM.Services.Models;
using APP.API.Auth;

namespace APP.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
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
                bool userNameExists = !string.IsNullOrWhiteSpace(model.UserName) && await _userService.ExistsByUserName(model.UserName);

                if (emailExists || userNameExists)
                {
                    if (emailExists && userNameExists)
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
                    user_name = string.IsNullOrWhiteSpace(model.UserName) ? model.Mail : model.UserName, // prefer provided username
                    name_surname = model.UserName,
                    user_type = 1,
                    last_login_time = DateTime.MinValue,
                    created_date = DateTime.Now,
                    modified_date = DateTime.Now,
                    is_active = true,
                    is_deleted = false,
                    is_registration = false
                };

                // additional optional fields can be stored in extended table or ignored for now

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


        [HttpPost("info")]
        [Authorize]
        public async Task<IActionResult> CreateInfo([FromBody] CreateUserInfoRequest model)
        {
            try
            {
                if (model == null || model.UserId == Guid.Empty)
                    return BadRequest(new { message = "Kullanıcı bilgisi gerekli." });

                var info = new user_inf
                {
                    id = Guid.NewGuid(),
                    user_id = model.UserId,
                    gender = model.Gender,
                    age_range = model.AgeRange,
                    daily_desk_hours = model.DailyDeskHours,
                    work_environment = model.WorkEnvironment,
                    working_position = model.WorkingPosition,
                    pain_areas = model.PainAreas != null ? string.Join(',', model.PainAreas) : string.Empty,
                    exercise_time = model.ExerciseTime,
                    exercise_types = model.ExerciseTypes != null ? string.Join(',', model.ExerciseTypes) : string.Empty,
                    reminder_preference = model.ReminderPreference,
                    created_date = DateTime.Now,
                    modified_date = DateTime.Now,
                    is_active = true,
                    is_deleted = false
                };

                var added = await _userService.AddUserInfo(info);
                if (added == null)
                    return Conflict(new { message = "Bilgiler kaydedilemedi." });

                return Ok(added);
            }
            catch (Exception ex)
            {
                return Problem(detail: "Genel Hata: " + ex.Message, statusCode: 500);
            }
        }

    }
}
