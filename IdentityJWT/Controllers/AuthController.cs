using IdentityJWT.Filters.ActionFilters;
using IdentityJWT.Models.DTO;
using IdentityJWT.Models;
using IdentityJWT.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using IdentityJWT.DataAccess.IRepo;
using IdentityJWT.Utility.Interface;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace IdentityJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(Auth_ConfirmJtiNotBlacklistedFilterAttribute))]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthController> _logger;
        private readonly IJwtManager _jwtManager;
        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork, ILogger<AuthController> logger, IJwtManager jwtManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _jwtManager = jwtManager;
            _signInManager = signInManager;
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
                _logger.LogInformation($"New user {registrationVM.Fname + registrationVM.Lname} created successfully.");
            }
            else
            {
                var errors = createUserResult.Errors.Select(e => e.Description);
                var errorString = string.Join(", ", errors);
                await _unitOfWork.EnterpriseLogging.Add(new EnterpriseLogging { App = "IdentityJWT", Area = "Auth", Note = $"User {newUser.Fname + " " + newUser.Lname} has failed to be created.", StackTrace = errorString, CreatedDate = DateTime.UtcNow, CorrelationID = correlationID });
                _logger.LogInformation($"User registration of {registrationVM.Fname + registrationVM.Lname} had an issue.");
                await _unitOfWork.Save();
                return BadRequest(new { errors = createUserResult.Errors, user = registrationVM} );
            }
            await _unitOfWork.Save();

            //generate a token to send back to the user along with a refresh token
            UserVM userVM = registrationVM.GetUserVM();
            return Created(string.Empty, userVM );
        }


        [HttpPost("login")]
        [AllowAnonymous]
        [GetGuidForLogging]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            var user = await _signInManager.UserManager.FindByEmailAsync(login.Email);
            if (user == null)
            {
                _logger.LogInformation($"User {login.Email} attempted to log in but was not found by signInManger");
                return Unauthorized("Username or Password is incorrect.");
            }

            var passwordCheck = await _signInManager.UserManager.CheckPasswordAsync(user, login.Password);
            if (!passwordCheck)
            {
                _logger.LogInformation($"User {login.Email} attempted to log in had the wrong password.");
                return Unauthorized("Username or Password is incorrect.");
            }

            if (!user.IsActive)
            {
                return BadRequest("Account has been deactivated. Please reach out to an Administrator.");
            }

            _logger.LogInformation($"User {user.Email} is logging in and generating JWT Token");
            // Generate your JWT token here
            var roles = await _signInManager.UserManager.GetRolesAsync(user);
            
            (string token, string refreshToken) = _jwtManager.GenerateJwtandRefreshToken(user, roles.ToList());
            JWTRefreshToken refreshTokenObject = new JWTRefreshToken(refreshToken, user.Id);
            
            await _unitOfWork.JwtRefreshToken.Add(refreshTokenObject);
            await _unitOfWork.Save();
            LoginResult loginResult = new LoginResult(user,token, refreshToken);

            return Ok(loginResult);
        }

        [HttpDelete("logout")]
        [GetGuidForLogging]
        public async Task<IActionResult> Logout()
        {
            string correlationID = HttpContext.Items["correlationID"].ToString() ?? "";
            var userID = HttpContext.User.FindFirstValue("userID");
            var jti = HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Jti);
            long.TryParse(HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Iat), out long issuedAt);
            long.TryParse(HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Exp), out long expiredAt);
            
            await _unitOfWork.JwtRefreshToken.DeleteAllRefreshTokensByUserID(userID);

            var authorizationHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            string jwtToken = null;
            if (authorizationHeader != null && authorizationHeader.StartsWith("Bearer "))
            {
                jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();
            }

            JwtBlacklistToken jwtBlacklist = new JwtBlacklistToken(jti, jwtToken, userID, (DateTimeOffset.FromUnixTimeSeconds(issuedAt)).UtcDateTime, (DateTimeOffset.FromUnixTimeSeconds(expiredAt)).UtcDateTime);
            await _unitOfWork.JwtBlacklistToken.Add(jwtBlacklist);
            _logger.LogInformation($"Logging out {userID}");
            await _unitOfWork.EnterpriseLogging.Add(new EnterpriseLogging { App = "IdentityJWT", Area = "Auth", Note = $"Logging out {userID}", CreatedDate = DateTime.UtcNow, CorrelationID = correlationID ?? Guid.NewGuid().ToString() });
            await _unitOfWork.Save();
            return Ok(new { message = "Logout Successful." });
        }

        [HttpPost("refreshToken")]
        [AllowAnonymous]
        [GetGuidForLogging]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            if(string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest("Invalid Request");
            }

            string correlationID = HttpContext.Items["correlationID"].ToString() ?? "";
            bool refreshTokenValid = await _jwtManager.RefreshTokenValidate(refreshToken);
            
            var refreshTokenObj = _unitOfWork.JwtRefreshToken.Get(t => t.Token.Equals(refreshToken));
            if (refreshTokenObj == null)
            {
                return BadRequest("Invalid refresh token.");
            }
            //deleting the refreshToken so that when we use it we can get rid of it so it cant be reused
            await _unitOfWork.JwtRefreshToken.Remove(refreshTokenObj);

            var user = await _signInManager.UserManager.FindByIdAsync(refreshTokenObj.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var roles = await _signInManager.UserManager.GetRolesAsync(user);
            (string token, string newRefreshToken) = _jwtManager.GenerateJwtandRefreshToken(user, roles.ToList());
            JWTRefreshToken refreshTokenObject = new JWTRefreshToken(newRefreshToken, user.Id);

            await _unitOfWork.JwtRefreshToken.Add(refreshTokenObject);
            _logger.LogInformation($"Removed old refresh token for {user.UserName} and added new one");
            await _unitOfWork.EnterpriseLogging.Add(new EnterpriseLogging { App = "IdentityJWT", Area = "Auth", Note = $"Removed old refresh token for {user.UserName} and added new one", CreatedDate = DateTime.UtcNow, CorrelationID = correlationID ?? Guid.NewGuid().ToString() });
            await _unitOfWork.Save();


            return Ok(new { Token = token, refreshToken = newRefreshToken });
        }

        [HttpGet("checkUser")]
        public async Task<IActionResult> CheckUserStatus()
        {
            ApplicationUser currentUser = new ApplicationUser();
            var userID = HttpContext.User.FindFirstValue("userID");
            if (userID != null)
            {
                currentUser = await _signInManager.UserManager.FindByIdAsync(userID);
            }
            else
            {
                return Forbid("Access Denied");
            }

            return Ok(new { message = "Confirmed User", user = currentUser.GetUserVM() });
        }

    }
}
