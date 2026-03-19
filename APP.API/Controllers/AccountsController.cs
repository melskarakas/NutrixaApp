using APP.API.Auth;
using ORM.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;
using ORM.Models.Models;

namespace APP.API.Controllers
{
    [AuthorizeAttribute]
    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountsController : ControllerBase
    {
        private readonly IUserService _userService;
        public AccountsController(IUserService userService)
        {
            _userService = userService;
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
        public async Task<ObjectResult> Update([FromBody] users user)
        {
            try
            {
                var res = await _userService.Update(user);
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
