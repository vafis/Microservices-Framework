using Eid.Microservices.BackChannelLoggerService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eid.Microservices.Core;
using Eid.Microservices.Core.Options;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

namespace Eid.Microservices.EidJwtBearerGrantMiddleware
{
    public class EidJwtBearerGrantMiddleware
    {
        private const string AccessTokenResponseKey = "AccessTokenResponseKey";

        private readonly RequestDelegate _next;
        private readonly IHttpClientJwtBearerGrant _httpClientJwtBearerGrant;
        private readonly JwtBearerGrantOptions _jwtBearerGrantOptions;
        private IMemoryCache _cache;
        private readonly IBackChannelLoggerService<EidJwtBearerGrantMiddleware> _backChannelLoggerService;
        private readonly IBearerTokenBuilder _bearerTokenBuilder;
        private string _refreshToken;

        public EidJwtBearerGrantMiddleware(RequestDelegate next, IHttpClientJwtBearerGrant httpClient,
            IOptions<JwtBearerGrantOptions> jwtBearerGrantOptions,
            IMemoryCache memoryCache,
            IBackChannelLoggerService<EidJwtBearerGrantMiddleware> backChannelLoggerService,
            IBearerTokenBuilder bearerTokenBuilder)
        {
            _next = next;
            _httpClientJwtBearerGrant = httpClient;
            _jwtBearerGrantOptions = jwtBearerGrantOptions.Value;
            _cache = memoryCache;
            _backChannelLoggerService = backChannelLoggerService;
            _bearerTokenBuilder = bearerTokenBuilder;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            AccessTokenResponse obj;
            if (!_cache.TryGetValue<AccessTokenResponse>(CacheKeys.AccessTokenResponse, out obj))
            {
                var assertion = _bearerTokenBuilder.BuildToken();

                var values = new Dictionary<string, string>
                {
                    {OAuth2Constants.GrantType, OAuth2Constants.GrantTypes.JWTBearerGrant},
                    {OAuth2Constants.Assertion, Convert.ToBase64String(Encoding.UTF8.GetBytes(assertion))}
                };

                await _httpClientJwtBearerGrant.RequestToken(values)
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
                            Task.Run(() => _backChannelLoggerService.LogWarning(x.Result));
                            return;
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
                RefreshToken = _refreshToken = json["refresh_token"].ToString(),
                //IdToken = json["id_token"].ToString(),
                Id = json["id"].ToString(),
                GrantType = OAuth2Constants.GrantTypes.JWTBearerGrant
            };
            //Declare & Set Expiration Time
            var expirationTime = DateTime.UtcNow.Add(_jwtBearerGrantOptions.TokenExpirationTime);
            //Declare & Set CancellationChangeToken
            var expirationToken = new CancellationChangeToken(
                new CancellationTokenSource(_jwtBearerGrantOptions.TokenExpirationTime).Token);
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
                {OAuth2Constants.ClientId, _jwtBearerGrantOptions.ClientId},
                {OAuth2Constants.Secret, _jwtBearerGrantOptions.Secret },
                {OAuth2Constants.GrantType, OAuth2Constants.GrantTypes.RefreshToken},
                {OAuth2Constants.RefreshToken, _refreshToken}
            };

            await _httpClientJwtBearerGrant.RefreshToken(values)
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
