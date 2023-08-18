using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;

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
                || !context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!context.HttpContext.Request.RouteValues.TryGetValue(RouteField, out var value)
                 || !context.HttpContext.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier && c.Value == value?.ToString()))
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
