using IdentityJWT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace IdentityJWT.Filters.ActionFilters
{
    public class GetGuidForLogging : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            //context.ActionArguments.Add("correlationID", Guid.NewGuid().ToString()); //no longer need to pass this as an parameter to method
            context.HttpContext.Items.Add("correlationID", Guid.NewGuid().ToString());
            
        }
    }
}
