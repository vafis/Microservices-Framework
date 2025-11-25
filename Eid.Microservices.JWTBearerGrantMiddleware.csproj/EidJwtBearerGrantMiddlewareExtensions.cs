using System;
using System.Collections.Generic;
using System.Text;
using Eid.Microservices.Core;
using Eid.Microservices.Core.Extensions;
using Eid.Microservices.Core.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eid.Microservices.EidJwtBearerGrantMiddleware
{
    public static class EidJwtBearerGrantMiddlewareExtensions
    {
        public static IServiceCollection AddJwtBearerGrantAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.Configure<JwtBearerGrantOptions>(configuration.GetSection("Secrets:Application:JWTBearerGrant"));
            services.AddTransient<IBearerTokenBuilder, BearerTokenBuilder>();
            services
                .AddHttpClientJwtBearerGrant<IHttpClientJwtBearerGrant, HttpClientJwtBearerGrant, HttpClientOptions>(
                    configuration, "EidApiExtension:HttpClientRequestToken");
            return services;
        }

        public static IApplicationBuilder UseJwtBearerGrantAuthentication(this IApplicationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.UseMiddleware<EidJwtBearerGrantMiddleware>();
        }
    }
}
