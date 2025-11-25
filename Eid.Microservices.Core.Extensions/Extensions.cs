using Eid.Microservices.Core.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace Eid.Microservices.Core.Extensions
{
    public static class Extensions
    {
 
        /// <summary>
        /// Registers  HttpClient EidRequestToken into Container 
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <typeparam name="TClientOptions"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="configurationSectionName"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpClientEidRequestToken<TClient, TImplementation, TClientOptions>(
            this IServiceCollection services,
            IConfiguration configuration,
            string configurationSectionName)
            where TClient : class
            where TImplementation : class, TClient
            where TClientOptions : HttpClientOptions, new() =>
            services
                .Configure<TClientOptions>(configuration.GetSection(configurationSectionName))
                .AddHttpClient<TClient, TImplementation>()
                .ConfigureHttpClient((sp, options) =>
                {
                    var httpClientOptions = sp.GetRequiredService<IOptions<TClientOptions>>().Value;
                    options.BaseAddress = httpClientOptions.BaseAddress;
                    options.Timeout = httpClientOptions.TimeOut;
                    options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(httpClientOptions.DefaultRequestHeadersAccept));
                })
                .ConfigurePrimaryHttpMessageHandler(_ => new DefaultHttpClientHandler())
                .AddHttpMessageHandler<EidRequestTokenDelegatingHandler>()
                .Services;
        /// <summary>
        /// Register  HttpClient ApiExtension into Container 
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <typeparam name="TClientOptions"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="configurationSectionName"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpClientApiExtension<TClient, TImplementation, TClientOptions>(
            this IServiceCollection services,
            IConfiguration configuration,
            string configurationSectionName)
            where TClient : class
            where TImplementation : class, TClient
            where TClientOptions : HttpClientOptions, new() =>
            services
                .Configure<TClientOptions>(configuration.GetSection(configurationSectionName))
                .AddHttpClient<TClient, TImplementation>()
                .ConfigureHttpClient((sp, options) =>
                {
                    var httpClientOptions = sp.GetRequiredService<IOptions<TClientOptions>>().Value;
                    options.BaseAddress = httpClientOptions.BaseAddress;
                    options.Timeout = httpClientOptions.TimeOut;
                    options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(httpClientOptions.DefaultRequestHeadersAccept));
                })
                .ConfigurePrimaryHttpMessageHandler(_ => new DefaultHttpClientHandler())
                .AddHttpMessageHandler<EidApiExtensionDelegatingHandler>()
                .Services;

        /// <summary>
        /// Registers  HttpClient JwtBearerGrant into Container 
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <typeparam name="TClientOptions"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="configurationSectionName"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpClientJwtBearerGrant<TClient, TImplementation, TClientOptions>(
            this IServiceCollection services,
            IConfiguration configuration,
            string configurationSectionName)
            where TClient : class
            where TImplementation : class, TClient
            where TClientOptions : HttpClientOptions, new() =>
            services
                .Configure<TClientOptions>(configuration.GetSection(configurationSectionName))
                .AddHttpClient<TClient, TImplementation>()
                .ConfigureHttpClient((sp, options) =>
                {
                    var httpClientOptions = sp.GetRequiredService<IOptions<TClientOptions>>().Value;
                    options.BaseAddress = httpClientOptions.BaseAddress;
                    options.Timeout = httpClientOptions.TimeOut;
                    options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(httpClientOptions.DefaultRequestHeadersAccept));
                })
                .ConfigurePrimaryHttpMessageHandler(_ => new DefaultHttpClientHandler())
                .AddHttpMessageHandler<EidJwtBearerGrantDelegatingHandler>()
                .Services;

        public static IServiceCollection AddHttpClient<TClient, TImplementation, TClientOptions>(
            this IServiceCollection services,
            IConfiguration configuration,
            string configurationSectionName)
            where TClient : class
            where TImplementation : class, TClient
            where TClientOptions : HttpClientOptions, new() =>
            services
                .Configure<TClientOptions>(configuration.GetSection(configurationSectionName))
                .AddHttpClient<TClient, TImplementation>()
                .ConfigureHttpClient((sp, options) =>
                {
                    var httpClientOptions = sp.GetRequiredService<IOptions<TClientOptions>>().Value;
                    options.BaseAddress = httpClientOptions.BaseAddress;
                    options.Timeout = httpClientOptions.TimeOut;
                    options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(httpClientOptions.DefaultRequestHeadersAccept));
                })
                .ConfigurePrimaryHttpMessageHandler(_ => new DefaultHttpClientHandler())
                .AddHttpMessageHandler<EidSecurityDelegatingHandler>()
                .Services;








    }

}
