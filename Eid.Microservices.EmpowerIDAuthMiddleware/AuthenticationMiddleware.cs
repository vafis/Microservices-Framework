using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Eid.Microservices.BackChannelLoggerService;
using Eid.Microservices.Core.Infrastructure;
using Eid.Microservices.Core.Models;
using Eid.Microservices.Core.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eid.Microservices.AuthentiicationMiddleware
{
    public class AuthenticationMiddleware
    {
        public readonly RequestDelegate _next;
        public readonly IHostingEnvironment _environment;
        public readonly IConfiguration _configuration;
        public readonly ApplicationCoreOptions _applicationCoreOptions;
        private readonly ILogger<AuthenticationMiddleware> _logger;

        public AuthenticationMiddleware(RequestDelegate next, 
                                        IHostingEnvironment environment,
                                        IOptions<ApplicationCoreOptions> applicationCoreOptions,
                                        ILogger<AuthenticationMiddleware> logger)
        {
            _next = next;
            _environment = environment;
            _applicationCoreOptions = applicationCoreOptions.Value;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User == null ||
                context.User.Identity == null ||
                !context.User.Identity.IsAuthenticated)
            {
                if (_environment.IsDevelopment() && !_applicationCoreOptions.EnableAuthentication)
                {
                    _logger.LogWarning("EmpowerId Authentication is DISABLED !");
                    await _next(context);
                }
                else
                {
                    
                    await context.ChallengeAsync(JwtBearerDefaults.AuthenticationScheme);
                    
                    var request = context.Request;
                    var currentUser = context.User.Identity.Name;
                    string controller = context.Request.HttpContext.GetRouteData().Values["controller"].ToString();
                    string action = context.Request.HttpContext.GetRouteData().Values["action"].ToString();

                    var anonymous = Singleton<List<AnonymousControllerActions>>.Instance;
                    var exists = anonymous.Find(x => x.Controller == controller + "Controller" && x.Actions.Find(a => a == action).Any());
                    if (exists == null)
                    {
                        context.Response.StatusCode = 401;
                        return;
                    }
                    //await context.AuthenticateAsync(AnonymousAuthOptions.DefaultScheme);
                    await _next(context);



                }

            }
            else
            {
                await _next(context);
            };
        }
    }
}
