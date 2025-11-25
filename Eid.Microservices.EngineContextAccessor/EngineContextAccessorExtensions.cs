using System;
using System.Collections.Generic;
using System.Text;
using Eid.Microservices.Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Eid.Microservices.EngineContextAccessor
{
    public static class EngineContextAccessorExtensions
    {
        public static IServiceCollection AddEngineContextAccessor(
            this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<ITypeFinder, AppDomainTypeFinder>();
            services.AddSingleton<IEngineContextAccessor, EngineContextAccessor>();
            return services;
        }
    }
}
