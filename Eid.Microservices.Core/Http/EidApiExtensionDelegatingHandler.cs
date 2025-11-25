using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eid.Microservices.Core.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Eid.Microservices.Core.Http
{
    public class EidApiExtensionDelegatingHandler : DelegatingHandler
    {
        private const string ApiKeyHeader = "X-EmpowerID-API-Key";
        private readonly JwtBearerGrantOptions _jwtBearerGrantOptions;
        private readonly PasswordGrantOptions _passwordGrantOptions;
        private readonly IMemoryCache _cache;

        public EidApiExtensionDelegatingHandler(IOptions<JwtBearerGrantOptions> jwtBearerGrantOptions,
                                                IOptions<PasswordGrantOptions> passwordGrantOptions,
                                                IMemoryCache cache)
        {
            _jwtBearerGrantOptions = jwtBearerGrantOptions.Value;
            _passwordGrantOptions = passwordGrantOptions.Value;
            _cache = cache;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessTokenResponse = _cache.Get<AccessTokenResponse>(CacheKeys.AccessTokenResponse);

            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessTokenResponse.AccessToken);
            //request.Headers.Add(ApiKeyHeader, _jwtBearerGrantOptions.ApiKey);
            if (accessTokenResponse.GrantType == OAuth2Constants.GrantTypes.JWTBearerGrant)
            {
                request.Headers.Add(ApiKeyHeader, _jwtBearerGrantOptions.ApiKey);
            }
            else { request.Headers.Add(ApiKeyHeader, _passwordGrantOptions.ApiKey); }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
