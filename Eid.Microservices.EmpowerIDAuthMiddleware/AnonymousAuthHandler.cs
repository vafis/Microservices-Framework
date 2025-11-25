using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Eid.Microservices.Core.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Claims;

namespace Eid.Microservices.AuthentiicationMiddleware
{
    public class AnonymousAuthHandler : AuthenticationHandler<AnonymousAuthOptions>
    {
        public AnonymousAuthHandler(IOptionsMonitor<AnonymousAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override  Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Context.User.Identity.IsAuthenticated==true && 
                Context.User.Identity.AuthenticationType == "AuthenticationTypes.Federation" )
            {
                return  Task.FromResult(AuthenticateResult.Success(
                   new AuthenticationTicket(new System.Security.Claims.ClaimsPrincipal(Context.User.Identity), JwtBearerDefaults.AuthenticationScheme)));
            }
            
            var claims = new[] {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "AnonymousUser"),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "Anonymous"),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role,"AllowAnonymous"),
                new System.Security.Claims.Claim("ControllerName", Request.HttpContext.GetRouteData().Values["controller"].ToString()),
                new System.Security.Claims.Claim("ActionName", Request.HttpContext.GetRouteData().Values["action"].ToString()),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Sid,AnonymousAuthOptions.DefaultSid), 
            };
           
            var identity = new System.Security.Claims.ClaimsIdentity(claims, Scheme.Name);
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            Context.SignInAsync(AnonymousAuthOptions.DefaultScheme,  principal, new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddMinutes(1),
                IsPersistent = false,
                AllowRefresh = false
            });
           // Context.User = principal;
            

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
