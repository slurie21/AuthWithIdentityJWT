using IdentityJWT.Filters.ActionFilters;
using IdentityJWT.Models.DTO;
using IdentityJWT.Models.ViewModels;
using IdentityJWT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IdentityJWT.DataAccess.IRepo;

namespace IdentityJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(Auth_ConfirmJtiNotBlacklistedFilterAttribute))]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [GetGuidForLogging]
        public async Task<IActionResult> Register([FromBody] RegistrationVM registrationVM)
        {
            string correlationID = HttpContext.Items["correlationID"].ToString() ?? "";

            _logger.LogInformation($"Starting process of creating new user and assigning roles. Correlation ID: {correlationID}");
            ApplicationUser newUser = new ApplicationUser(registrationVM);

            var createUserResult = await _userManager.CreateAsync(newUser, registrationVM.Password);
            if (createUserResult.Succeeded)
            {
                registrationVM.Id = newUser.Id;
                await _unitOfWork.EnterpriseLogging.Add(new EnterpriseLogging { App = "IdentityJWT", Area = "Auth", Note = $"User {newUser.Fname + " " + newUser.Lname} has been created successfully.", CreatedDate = DateTime.UtcNow, CorrelationID = correlationID });
                _logger.LogInformation($"New user {registrationVM.Fname + " " + registrationVM.Lname} created successfully.");
            }
            else
            {
                var errors = createUserResult.Errors.Select(e => e.Description);
                var errorString = string.Join(", ", errors);
                await _unitOfWork.EnterpriseLogging.Add(new EnterpriseLogging { App = "IdentityJWT", Area = "Auth", Note = $"User {newUser.Fname + " " + newUser.Lname} has failed to be created.", StackTrace = errorString, CreatedDate = DateTime.UtcNow, CorrelationID = correlationID });
                _logger.LogInformation($"User registration of {registrationVM.Fname + " " + registrationVM.Lname} had an issue.");
                await _unitOfWork.Save();
                return BadRequest(new { errors = createUserResult.Errors, user = registrationVM });
            }
            await _unitOfWork.Save();

            //generate a token to send back to the user along with a refresh token
            UserVM userVM = registrationVM.GetUserVM();
            return Created(string.Empty, userVM);
        }
    }
}
