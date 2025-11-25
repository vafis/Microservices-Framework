using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Eid.Microservices.Administration.HttpClients;
using Eid.Microservices.Administration.Services;
using Eid.Microservices.AuthContextService;
using Eid.Microservices.AuthentiicationMiddleware;
using Eid.Microservices.BackChannelLoggerService;
using Eid.Microservices.Core.Extensions;
using Eid.Microservices.Core.Http;
using Eid.Microservices.Core.Infrastructure;
using Eid.Microservices.Core.Options;
using Eid.Microservices.Core.Security;
using Eid.Microservices.EidJwtBearerGrantMiddleware;
using Eid.Microservices.EidRbacAuthorizationServiceFilter;
using Eid.Microservices.EidValidateRequestMiddleware;
using Eid.Microservices.EmbeddedResourceReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eid.Microservices.Core.Engine
{
    public class Engine : IEngine
    {
        private readonly ITypeFinder _appDomainTypeFinder;
        public Engine(ITypeFinder appDomainTypeFinder )
        {
            _appDomainTypeFinder = appDomainTypeFinder;
        }

        private IServiceProvider _serviceProvider { get; set; }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            //Options
            services.Configure<ApplicationCoreOptions>(configuration.GetSection("ApplicationBase:Core"));
            services.Configure<MemoryCacheOptions>(configuration.GetSection("ApplicationBase:MemoryCacheOptions"));
            services.Configure<AuthContextOverrides>(configuration.GetSection("ApplicationBase:AuthContextOverrides"));

            services.AddTransient<IEmbeddedResourceReader, EmbeddedResourceReader.EmbeddedResourceReader>();

            services.AddAuthContext(configuration);
           // services.AddScoped<IAuthContext, HttpAuthContext>();



            //KOSTAS
            //TODO : ADDSCOPED IBackChannelLoggerService !!!!
            services.AddTransient(typeof(IBackChannelLoggerService<>), typeof(BackChannelLoggerService<>));

            //**********------ Rbac Authorization Check ----------------------------***********
            services
                .AddHttpClientApiExtension<IHttpClientRbacAuthorization, HttpClientRbacAuthorization, HttpClientOptions>(
                    configuration, "EidCore:HttpClient");
            services.AddScoped<RbacAuthorizationServiceFilter>();
            //**********------------------------------------------------------------***********

            //Handlers
            services.AddTransient<DefaultHttpClientHandler>();
            services.AddTransient<EidRequestTokenDelegatingHandler>();
            services.AddTransient<EidApiExtensionDelegatingHandler>();
            services.AddTransient<EidSecurityDelegatingHandler>();
            services.AddTransient<EidJwtBearerGrantDelegatingHandler>();

            //Administration Application
            services
                .AddHttpClientApiExtension<IHttpClientAdmin, HttpClientAdmin, HttpClientOptions>(
                    configuration, "EidCore:HttpClient");
            services.AddTransient<IServiceFilterResolverService, ServiceFilterResolverService>();
            services.AddMvcCore().AddApplicationPart(Assembly.Load(new AssemblyName("Eid.Microservices.Administration")));


            services.AddSingleton<IAuthorizationHandler, AllowAnonymousHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy(nameof(Policy.AllowAnonymous),
                    builder =>
                    {
                        builder.RequireAuthenticatedUser();
                        builder.AuthenticationSchemes.Add("Anonymous_Auth");
                        builder.Requirements.Add(new AllowAnonymousRequirement(AnonymousAuthOptions.DefaultSid));
                    });
            });


            //0f8fad5b-d9cb-469f-a165-70867728950e

            services.AddEmpowerIdAuthentication(configuration);
            ////services.AddEidRequestToken(this.Configuration);
            services.AddJwtBearerGrantAuthentication(configuration);
            services.AddEidValidateRequest(configuration);



            RunStartupTasks();

            //Set Service Provider
            _serviceProvider = services.BuildServiceProvider();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseEndpointRouting();
            app.UseEmpowerIdAuthentication();
            //app.UseEidRequestToken();
            app.UseJwtBearerGrantAuthentication();
            app.UseEidValidateRequest();
        }


        private void RunStartupTasks()
        {
            
            var startupTasks = _appDomainTypeFinder.FindClassesOfType<IStartupTask>();

            var instances = startupTasks
                .Select(startupTask => (IStartupTask)Activator.CreateInstance(startupTask))
                .OrderBy(startupTask => startupTask.Order);

            //execute tasks
            instances.ToList().ForEach(x=>x.Execute());


        }




    }
}
