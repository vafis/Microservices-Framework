using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Eid.Microservices.Core;
using Eid.Microservices.EidRequestTokenMiddleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Eid.Microservices.Core.Extensions;
using Eid.Microservices.Core.Options;

//Install-Package Microsoft.Extensions.Options.ConfigurationExtensions -Version 2.2.0
namespace Eid.Microservices.EidRequestTokenMiddleware
{
    public static class EidRequestTokenMiddlewareExtensions
    {
        public static IServiceCollection AddEidRequestToken(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.Configure<PasswordGrantOptions>(configuration.GetSection("Secrets:Application:PasswordGrant"));
            services
                .AddHttpClientEidRequestToken<IHttpClientRequestToken, HttpClientRequestToken, HttpClientOptions>(
                    configuration, "EidApiExtension:HttpClientRequestToken");
            return services;
        }

        public static IApplicationBuilder UseEidRequestToken(this IApplicationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.UseMiddleware<EidRequestTokenMiddleware>();
        }
    }
}
