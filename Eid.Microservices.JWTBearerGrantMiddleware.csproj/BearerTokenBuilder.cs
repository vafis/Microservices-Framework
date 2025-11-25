using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Schema;
using Eid.Microservices.Core;
using Eid.Microservices.Core.Options;
using Eid.Microservices.EmbeddedResourceReader;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace Eid.Microservices.EidJwtBearerGrantMiddleware
{
    public class BearerTokenBuilder : IBearerTokenBuilder
    {
        //private const string EmbeddedResourceQualifier = "Eid.Microservices.JWTBearerGrantMiddleware";
        private const string EmbeddedResourceName = "EmpowerID API Extension Certificate.pfx";

        private readonly List<Claim> _claims = new List<Claim>();
        private readonly JwtBearerGrantOptions _jwtBearerGrantOptions;
        private readonly DateTime _notBefore = DateTime.UtcNow;
        private readonly IEmbeddedResourceReader _embeddedResourceReader;
        private X509Certificate2 _signingCertificate;
        private readonly ILogger _logger;
        private readonly JwtSecurityTokenHandler _securityTokenHandler = new JwtSecurityTokenHandler();
        private readonly ApplicationCoreOptions _applicationCoreOptions;
        private IMemoryCache _cache;

        public BearerTokenBuilder(IOptions<JwtBearerGrantOptions> jwtBearerGrantOptions,
                                  IEmbeddedResourceReader embeddedResourceReader,
                                  ILogger<BearerTokenBuilder> logger,
                                  IOptions<ApplicationCoreOptions> applicationCoreOptions,
                                  IMemoryCache memoryCache)
        {
            _jwtBearerGrantOptions = jwtBearerGrantOptions.Value;
            _embeddedResourceReader = embeddedResourceReader;
            _logger = logger;
            _applicationCoreOptions = applicationCoreOptions.Value;
            _cache = memoryCache;
        }

        public string BuildToken()
        {
            if (!_cache.TryGetValue<X509Certificate2>(CacheKeys.SigningCertificate, out _signingCertificate))
            {
                //Made it for unit/integration Tests 
                var asm = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(x => x.FullName.Contains(_applicationCoreOptions.EmbeddedResourceQualifier));
                var entryName = asm.GetName().Name;
                var resourceName = $"{entryName}.{EmbeddedResourceName}";

                _signingCertificate = _embeddedResourceReader.GetCertificate(asm.GetExportedTypes().FirstOrDefault(), resourceName,
                    _jwtBearerGrantOptions.Password);

                var cacheExpirationOptions = new MemoryCacheEntryOptions()
                {
                    Priority = CacheItemPriority.NeverRemove,
                    AbsoluteExpiration = DateTime.UtcNow.AddMonths(10)
                };
                _cache.Set<X509Certificate2>(CacheKeys.SigningCertificate, _signingCertificate, cacheExpirationOptions);
            }

            //Disabled and refactored for Testing proposes !!!!
            //var entryName = Assembly.GetEntryAssembly()?.GetName().Name;
            // var resourceName = $"{entryName}.{EmbeddedResourceName}";
            //_signingCertificate = _embeddedResourceReader.GetCertificate(Assembly.GetEntryAssembly()?.GetExportedTypes().FirstOrDefault(), resourceName, _jwtBearerGrantOptions.Password);

            if (_signingCertificate == null)
            {
                _logger.LogError("You must specify an X509 certificate to use for signing the JWT Token");
                throw new InvalidOperationException(
                    "You must specify an X509 certificate to use for signing the JWT Token");
            }

            var signingCredentials = new SigningCredentials(new X509SecurityKey(_signingCertificate), SecurityAlgorithms.RsaSha256);

            _claims.Add(new Claim("sub", _signingCertificate.Thumbprint));
            var identity = new ClaimsIdentity(_claims);

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                IssuedAt = DateTime.UtcNow,
                Audience = _jwtBearerGrantOptions.CallbackUrl,
                Issuer = _jwtBearerGrantOptions.ClientId,
                NotBefore = _notBefore,
                Expires = DateTime.UtcNow.Add(_jwtBearerGrantOptions.TokenExpirationTime),
                SigningCredentials = signingCredentials,
                Subject = identity
            };

            var token = _securityTokenHandler.CreateToken(securityTokenDescriptor);

            //Encoded Access Token
            return _securityTokenHandler.WriteToken(token);
        }
    }
}
