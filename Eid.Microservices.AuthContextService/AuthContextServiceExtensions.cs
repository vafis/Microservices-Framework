using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eid.Microservices.AuthContextService
{
    public static class AuthContextServiceExtensions
    {
        public static IServiceCollection AddAuthContext(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.Configure<AuthContextOverrides>(configuration.GetSection("ApplicationBase:AuthContextOverrides"));
            services.AddScoped<IAuthContext, HttpAuthContext>();

            return services;
        }
    }
}
