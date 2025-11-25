using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eid.Microservices.Core.Security
{
    public class AllowAnonymousHandler : AuthorizationHandler<AllowAnonymousRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AllowAnonymousRequirement requirement)
        {
            var identity = (ClaimsIdentity)context.User.Identity;
            if (identity.IsAuthenticated && identity.AuthenticationType == "AuthenticationTypes.Federation")
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.IsInRole("AllowAnonymous") &&
                context.User.HasClaim(c => c.Type == "ControllerName") &&
                context.User.HasClaim(c => c.Type ==  "ActionName"))
            {
                var sid = context.User
                    .FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid").Value;
                var controllerReq = context.User.FindFirst(c => c.Type == "ControllerName").Value;
                var actionReq = context.User.FindFirst(c => c.Type == "ActionName").Value;
                RouteData routeData = ((Microsoft.AspNetCore.Mvc.ActionContext) context.Resource).RouteData;

                if ( sid==requirement.Sid && controllerReq== routeData.Values["Controller"] && actionReq== routeData.Values["Action"]) 
                {
                    context.Succeed(requirement);
                }
                return Task.CompletedTask;
            }
            
            return Task.CompletedTask;
        }
    }
}
