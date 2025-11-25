using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eid.Microservices.BackChannelLoggerService;
using Eid.Microservices.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MemoryCacheOptions = Eid.Microservices.Core.Options.MemoryCacheOptions;

namespace Eid.Microservices.EidValidateRequestMiddleware
{
    public class EidValidateRequestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpClientEidValidateRequest _httpClient;
        private IMemoryCache _cache;
        private readonly MemoryCacheOptions _memoryCacheOptions;
        private readonly IBackChannelLoggerService<EidValidateRequestMiddleware> _backChannelLoggerService;


        public EidValidateRequestMiddleware(RequestDelegate next, 
                                            IHttpClientEidValidateRequest httpClient,
                                            IMemoryCache memoryCache,
                                            IOptions<MemoryCacheOptions> memoryCacheOptions,
                                            IBackChannelLoggerService<EidValidateRequestMiddleware> backChannelLoggerService)
        {
            _next = next;
            _httpClient = httpClient;
            _cache = memoryCache;
            _memoryCacheOptions = memoryCacheOptions.Value;
            _backChannelLoggerService = backChannelLoggerService;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            // Check for Http Proxy Servers or Load Balances Servers
            // The value is a comma+space separated list of IP addresses, the left-most being the original client,
            // and each successive proxy that passed the request adding the IP address where it received the request from

            string remoteIpAddress;
            if (context.Connection.RemoteIpAddress != null)
            {
                remoteIpAddress = context.Connection.RemoteIpAddress.ToString() == "::1"
                    ? "127.0.0.1" : context.Connection.RemoteIpAddress.MapToIPv4().ToString();
                if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
                {
                    remoteIpAddress = context.Request.Headers["X-Forwarded-For"];
                    remoteIpAddress.Split(',').ToList().Select(s => s.Trim()).FirstOrDefault();
                }
            }
            else
            {
                remoteIpAddress = "127.0.0.1";
            }

            var parameters = new Dictionary<string, string>() { { "hostname", remoteIpAddress } };

            string obj;
            if (_memoryCacheOptions.EidValidateRequestCacheOptions.Enable)
            {
                if (!_cache.TryGetValue<string>(CacheKeys.RemoteIpAddress + remoteIpAddress, out obj))
                {
                    var ret = await ValidateRequest(parameters);
                    AddToCache(CacheKeys.RemoteIpAddress + remoteIpAddress, ret);
                    if (!ret)
                    {
                        context.Response.StatusCode = 401;
                        return;
                    }
                    await _next(context);
                    return;
                }

                if (!bool.Parse(obj))
                {
                    context.Response.StatusCode = 401;
                    return;
                }
                await _next(context);
            }
            else
            {
                if (!await ValidateRequest(parameters))
                {
                    context.Response.StatusCode = 401;
                    return;
                }
                await _next(context);
            }
           
            
        }

        private void AddToCache(string key, bool value)
        {
            //Declare & Set Expiration Time
            var expirationTime = DateTime.UtcNow.Add(_memoryCacheOptions.EidValidateRequestCacheOptions.ExpirationTime);
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

            _cache.Set<string>(key, value.ToString(), cacheExpirationOptions);
        }

        private async Task<bool> ValidateRequest(Dictionary<string, string> parameters)
        {
            return await await _httpClient.ValidateRequest(parameters)
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
