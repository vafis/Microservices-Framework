using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Eid.Microservices.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Eid.Microservices.Core.Extensions;
using Eid.Microservices.Core.Options;

namespace Eid.Microservices.EidValidateRequestMiddleware
{
    public static class EidValidateRequestMiddlewareExtensions
    {
        public static IApplicationBuilder UseEidValidateRequest(this IApplicationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            return builder.UseMiddleware<EidValidateRequestMiddleware>();
        }

        public static IServiceCollection AddEidValidateRequest(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.Configure<PasswordGrantOptions>(configuration.GetSection("Secrets:Application:PasswordGrant"));
            services
                .AddHttpClientApiExtension<IHttpClientEidValidateRequest, HttpClientEidValidateRequest, HttpClientOptions> (
                    configuration, "EidCore:HttpClient");
             return services;
        }

    }
}
