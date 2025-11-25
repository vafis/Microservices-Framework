using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Eid.Microservices.Core.Options;
using Eid.Microservices.Core.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Eid.Microservices.AuthentiicationMiddleware
{
    public static class EmpowerIDAuthMiddlewareExtensions
    {

        public static IServiceCollection AddEmpowerIdAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.Configure<AuthenticationMiddlewareOptions>(configuration.GetSection("ApplicationBase:AuthenticationOptions"));
          
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddScheme<AnonymousAuthOptions, AnonymousAuthHandler>("Anonymous_Auth", _ => { })
                .AddJwtBearer(options =>
                {
                    var authenticationOptions = services.BuildServiceProvider().GetRequiredService<IOptions<AuthenticationMiddlewareOptions>>().Value;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // Validate the JWT Issuer (iss) claim
                        ValidateIssuer = true,
                        ValidIssuer = authenticationOptions.Issuer,
                        // Validate the JWT Audience (aud) claim
                        ValidateAudience = true,
                       // ValidAudiences = configuration.GetSection("ApplicationBase:AuthenticationOptions:Audiences").GetChildren().ToArray().Select(c => c.Value).ToList(),
                        ValidAudiences = authenticationOptions.ValidAudiences,
                        //ValidAudience = authenticationOptions.Audience,
                        // Validate the token expiry
                        ValidateLifetime = true,
                        // If you want to allow a certain amount of clock drift, set that here:
                        ClockSkew = TimeSpan.FromMinutes(5),
                        //Validate Signing Key 
                        ValidateIssuerSigningKey = true,
                        SaveSigninToken = true
                    };
                    options.MetadataAddress = authenticationOptions.MetadataAddress;
                    options.Events = new JwtBearerEvents()
                    {
                        OnChallenge = context => {
                           

                            //context.HandleResponse();

                            return Task.FromResult(0);
                        },
                        OnAuthenticationFailed = context => {
                            

                            return Task.FromResult(0);
                        },
                        OnMessageReceived = context => {
                           
                            return Task.FromResult(0);
                        },
                        OnTokenValidated = context => {
                           
                            return Task.FromResult(0);
                        }

                    };
                });

            return services;
        }

        public static IApplicationBuilder UseEmpowerIdAuthentication(this IApplicationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.UseAuthentication();
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
