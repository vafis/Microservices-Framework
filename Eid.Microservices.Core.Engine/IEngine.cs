using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eid.Microservices.Core.Engine
{
    public interface IEngine
    {
        void ConfigureServices(IServiceCollection services, IConfiguration configuration);
        void Configure(IApplicationBuilder app, IHostingEnvironment env);
    }
}
