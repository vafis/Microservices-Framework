using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eid.Microservices.BackChannelLoggerService;
using Eid.Microservices.Core;
using Eid.Microservices.Core.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Eid.Microservices.EidRequestTokenMiddleware
{
    public class EidRequestTokenMiddleware
    {
        private const string AccessTokenResponseKey = "AccessTokenResponseKey";

        private readonly RequestDelegate _next;
        private readonly IHttpClientRequestToken _httpClientRequestToken;
        private readonly PasswordGrantOptions _requestTokenOptions;
        private IMemoryCache _cache;
        private readonly IBackChannelLoggerService<EidRequestTokenMiddleware> _backChannelLoggerService;

        public EidRequestTokenMiddleware(RequestDelegate next, IHttpClientRequestToken httpClient, 
                                         IOptions<PasswordGrantOptions> requestTokenOptions,
                                         IMemoryCache memoryCache, 
                                         IBackChannelLoggerService<EidRequestTokenMiddleware> backChannelLoggerService)
        {
            _next = next;
            _httpClientRequestToken = httpClient;
            _requestTokenOptions = requestTokenOptions.Value;
            _cache = memoryCache;
            _backChannelLoggerService = backChannelLoggerService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            AccessTokenResponse obj;
            if (!_cache.TryGetValue<AccessTokenResponse>(CacheKeys.AccessTokenResponse, out obj))
            {
                var values = new Dictionary<string, string>
                {
                    {OAuth2Constants.ClientId, _requestTokenOptions.ClientId},
                    {OAuth2Constants.Secret, _requestTokenOptions.Secret },
                    {OAuth2Constants.GrantType, OAuth2Constants.GrantTypes.Password}
                };

                await _httpClientRequestToken.RequestToken(values)
                    .ContinueWith(x =>
                    {
                        if (x.IsFaulted)
                        {
                            if (x.Exception != null)
                                Task.Run(() => _backChannelLoggerService.LogError(x.Exception));
                        }
                        if (x.IsCanceled)
                        {
                            Task.Run(() => _backChannelLoggerService.LogError(new Exception("Current request was cancelled")));
                        }
                        if (!x.Result.IsSuccessStatusCode)
                        {
                            Task.Run(() => _backChannelLoggerService.LogWarning(x.Result));
                        }
                        if (x.IsCompleted && x.Result.IsSuccessStatusCode)
                        {
                            Task.Run(() =>
                                _backChannelLoggerService.LogInformation(
                                    "Request EmpowerId Token completed successfully!"));
                            AddToCacheAccessTokenResponse(x);
                        }
                    });
            }
            
            await _next(context);
        }

        private void AddToCacheAccessTokenResponse(Task<HttpResponseMessage> x)
        {
            var json = JObject.Parse(x.Result.Content.ReadAsStringAsync().Result);
            var accessTokenResponse = new AccessTokenResponse
            {
                AccessToken = json["access_token"].ToString(),
                TokenType = json["token_type"].ToString(),
                ExpiresIn = int.Parse(json["expires_in"].ToString()),
                RefreshToken = json["refresh_token"].ToString(),
                //IdToken = json["id_token"].ToString(),
                Id = json["id"].ToString(),
                GrantType = OAuth2Constants.GrantTypes.Password
            };
            //Declare & Set Expiration Time
            var expirationTime = DateTime.UtcNow.AddSeconds(accessTokenResponse.ExpiresIn - 300);
            //Declare & Set CancellationChangeToken
            var expirationToken = new CancellationChangeToken(
                new CancellationTokenSource(TimeSpan.FromSeconds(accessTokenResponse.ExpiresIn - 300)).Token);
            var cacheExpirationOptions = new MemoryCacheEntryOptions()
            {
                Priority = CacheItemPriority.NeverRemove,
                AbsoluteExpiration = expirationTime
            };
            //Force expiration through CancellationChangeToken
            cacheExpirationOptions.AddExpirationToken(expirationToken);
            //Register Callback
            cacheExpirationOptions.RegisterPostEvictionCallback(callback: CacheItemRemoved,
                state: this);

            _cache.Set<AccessTokenResponse>(CacheKeys.AccessTokenResponse, accessTokenResponse, cacheExpirationOptions);
        }

        private async void CacheItemRemoved(object key, object value, EvictionReason reason, object state)
        {
            var values = new Dictionary<string, string>
            {
                {OAuth2Constants.ClientId, _requestTokenOptions.ClientId},
                {OAuth2Constants.Secret, _requestTokenOptions.Secret },
                {OAuth2Constants.GrantType, OAuth2Constants.GrantTypes.RefreshToken},
                {OAuth2Constants.RefreshToken, ((AccessTokenResponse)value).RefreshToken}
            };
            await _httpClientRequestToken.RefreshToken(values)
                .ContinueWith(x =>
                {
                    if (x.IsFaulted)
                    {
                        if (x.Exception != null)
                            Task.Run(() => _backChannelLoggerService.LogError(x.Exception));
                        return;
                    }
                    if (x.IsCanceled)
                    {
                        Task.Run(() => _backChannelLoggerService.LogError(new Exception("Current request was cancelled")));
                        return;
                    }
                    if (!x.Result.IsSuccessStatusCode)
                    {
                        Task.Run(() => _backChannelLoggerService.LogWarning(x.Result.Content.ReadAsStringAsync().Result));
                        return;
                    }
                    if (x.IsCompleted && x.Result.IsSuccessStatusCode)
                    {
                        Task.Run(() => _backChannelLoggerService.LogInformation("Refresh Token Request Completed Successfully"));
                        AddToCacheAccessTokenResponse(x);
                    }
                });
        }
    }
}
