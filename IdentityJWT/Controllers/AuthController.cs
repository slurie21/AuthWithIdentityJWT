using IdentityJWT.Filters.ActionFilters;
using IdentityJWT.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace IdentityJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {

        [HttpPost("register")]
        [AllowAnonymous]
        [GetGuidForLogging] 
        public async Task<IActionResult> Register([FromBody] RegistrationVM registrationVM)
        {
            string correlationID = HttpContext.Items["correlationID"].ToString() ?? "";
            return Created(string.Empty, registrationVM);
        }

    }
}
