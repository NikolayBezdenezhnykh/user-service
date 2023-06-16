using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AllowCurrentUserAuthorization : Attribute, IAuthorizationFilter
    {
        public string RouteField { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
            {
                return;
            }

            if (context.HttpContext.User?.Identity == null
                || string.IsNullOrEmpty(context.HttpContext.User.Identity.Name)
                || !context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!context.HttpContext.Request.RouteValues.TryGetValue(RouteField, out var value)
                 || context.HttpContext.User.Identity.Name != value?.ToString())
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
