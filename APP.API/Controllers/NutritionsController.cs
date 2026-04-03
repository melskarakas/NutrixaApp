using APP.API.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APP.API.Controllers
{
    [AuthorizeAttribute]
    [ApiController]
    [Route("[controller]/[action]")]
    public class NutritionsController : ControllerBase
    {
    }
}
