using Eid.Microservices.AuthContextService;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eid.Microservices.Core;
using Microsoft.Extensions.Options;
using Eid.Microservices.Core.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Eid.Microservices.BackChannelLoggerService;

namespace Eid.Microservices.EidRbacAuthorizationServiceFilter
{
    public class RbacAuthorizationServiceFilter : IAsyncActionFilter //ActionFilterAttribute//, IResultFilter
    {
        private readonly IHttpClientRbacAuthorization _httpClientRbacAuthorization;
        private readonly IAuthContext _httpAuthContext;
       // private readonly ApplicationCoreOptions _applicationCoreOptions;
        private IMemoryCache _cache;
        private readonly MemoryCacheOptions _memoryCacheOptions;
        private readonly IBackChannelLoggerService<RbacAuthorizationServiceFilter> _backChannelLoggerService;

        public RbacAuthorizationServiceFilter(IHttpClientRbacAuthorization httpClientRbacAuthorization,
                                              IAuthContext httpAuthContext,
                                              //IOptions<ApplicationCoreOptions> applicationCoreOptions,
                                              IMemoryCache memoryCache,
                                              IOptions<MemoryCacheOptions> memoryCacheOptions,
                                              IBackChannelLoggerService<RbacAuthorizationServiceFilter> backChannelLoggerService)
        {
            _httpClientRbacAuthorization = httpClientRbacAuthorization;
            _httpAuthContext = httpAuthContext;
            //_applicationCoreOptions = applicationCoreOptions.Value;
            _cache = memoryCache;
            _memoryCacheOptions = memoryCacheOptions.Value;
            _backChannelLoggerService = backChannelLoggerService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string controllerName = "";
            string actionName = "";
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (descriptor != null)
            {
                controllerName = descriptor.ControllerTypeInfo.Name;
                actionName = descriptor.ActionName;
            }

            var resource =  controllerName + "." + actionName;

            var key = CacheKeys.RbacAuth + ":" + _httpAuthContext.PersonId + ":" + resource;

            bool hasaccessToResource ;
            if (_memoryCacheOptions.RbacAuthorizationCacheOptions.Enable && !_cache.TryGetValue<bool>(key, out hasaccessToResource))
            {
                hasaccessToResource = await RbackAuthorizeAsync(resource).ContinueWith(x =>
                {
                    AddToCache(key, x.Result);
                    return x.Result;
                });

                if (!hasaccessToResource)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                await next();
            }
            else
            {
                await RbackAuthorizeAsync(resource).ContinueWith(async x =>
                {
                    if (!x.Result)
                    {
                        context.Result = new UnauthorizedResult();
                        return;
                    }
                    await next();
                });
            }
        }

        private void AddToCache(string key , bool value)
        {
            //Declare & Set Expiration Time
            var expirationTime = DateTime.UtcNow.Add(_memoryCacheOptions.RbacAuthorizationCacheOptions.ExpirationTime);
            //Declare & Set CancellationChangeToken
            var expirationToken = new CancellationChangeToken(
                new CancellationTokenSource(_memoryCacheOptions.EidValidateRequestCacheOptions.ExpirationTime).Token);
            var cacheExpirationOptions = new MemoryCacheEntryOptions()
            {
                Priority = CacheItemPriority.Normal,
                AbsoluteExpiration = expirationTime
            };
            //Force expiration through CancellationChangeToken
            cacheExpirationOptions.AddExpirationToken(expirationToken);

            _cache.Set<bool>(key, value, cacheExpirationOptions);

        }

        private async Task<bool> RbackAuthorizeAsync(string resource)
        {
            var values = new Dictionary<string, string>
            {
                {"person", _httpAuthContext.PersonId.ToString()},
                {"resource", resource}
            };

            return await await _httpClientRbacAuthorization.RbackAuthorize(values)
                .ContinueWith(x =>
                {
                    if (x.IsFaulted)
                    {
                        if (x.Exception != null)
                            Task.Run(() => _backChannelLoggerService.LogError(x.Exception));
                        return Task.FromResult(false);
                    }
                    if (x.IsCanceled)
                    {
                        Task.Run(() => _backChannelLoggerService.LogError(new Exception("Current request was cancelled")));
                        return Task.FromResult(false);
                    }

                    return !x.Result.IsSuccessStatusCode ? Task.FromResult(false)
                        : Task.FromResult(JsonConvert.DeserializeObject<bool>(x.Result.Content.ReadAsStringAsync().Result));
                });
        }
    }
}
