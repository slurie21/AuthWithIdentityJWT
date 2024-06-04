using IdentityJWT.DataAccess.IRepo;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IdentityJWT.Filters.ActionFilters
{
    public class Auth_ConfirmJtiNotBlacklistedFilterAttribute : ActionFilterAttribute
    {
        private readonly IUnitOfWork _unitOfWork;

        public Auth_ConfirmJtiNotBlacklistedFilterAttribute(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }   
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var jti = context.HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Jti); //doesnt quite work.  need to get the value
            
            if (!string.IsNullOrEmpty(jti))
            {
                //var result  = _unitOfWork.JwtBlacklistToken.Get(x => x.Jti == jti);
                //if(result != null)
                //{
                //    context.Result = new UnauthorizedResult(); 
                //}
            }

        }
        
    }
}
