using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Eid.Microservices.AuthContextService
{
    public class HttpAuthContext : IAuthContext
    {
        private const string authorizationHeaderName = "Authorization";
        private const string attribClaimType = "attrib";

        private readonly HttpContext _httpContext;
        private readonly AuthContextOverrides _authContextOverrides;
        private readonly EidAttribClaimValues _attribValues;

        public HttpAuthContext(IHttpContextAccessor accessor, IOptions<AuthContextOverrides> authContextOverrides)
        {
            // this implemeantation of IAuthContext can not work wothout a http request, we need to thow an exception if missing
            _httpContext = accessor.HttpContext ?? throw new ArgumentNullException(nameof(HttpContext));
            _authContextOverrides = authContextOverrides.Value;

            _attribValues = GetAttributeValues();
        }

        public string RequestPath => _httpContext.Request.Path;

        public string QueryString => _httpContext.Request.QueryString.Value;

        public Guid PersonId
        {
            get
            {
                var result = _attribValues.PersonGuid;

                if (!string.IsNullOrEmpty(_authContextOverrides.PersonId))
                    Guid.TryParse(_authContextOverrides.PersonId, out result);

                return result;
            }
        }

        public Guid? TenantId
        {
            get
            {
                var result = _attribValues.TenantGuid;

                if (!string.IsNullOrEmpty(_authContextOverrides.TenantId))
                    Guid.TryParse(_authContextOverrides.TenantId, out result);

                return result == Guid.Empty ? (Guid?)null : result;
            }
        }

        public string DisplayName
        {
            get
            {
                var result = _attribValues.DisplayName;
                return string.IsNullOrEmpty(result) ? string.Empty : result;
            }
        }

        public string Email
        {
            get
            {
                var result = _attribValues.Email;
                return string.IsNullOrEmpty(result) ? string.Empty : result;
            }
        }

        public string Login
        {
            get
            {
                var result = GetUserClaim("sub");
                return string.IsNullOrEmpty(result) ? string.Empty : result;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                var result = IsRequestAuthenticated;

                if (!string.IsNullOrEmpty(_authContextOverrides.IsAuthenticated))
                    bool.TryParse(_authContextOverrides.IsAuthenticated, out result);

                return result;
            }
        }

        public string EIDAccessToken
        {
            get
            {
                var result = string.Empty;

                // only retrieve value if truly authenticated (not with an overriden IsAuthenticated property)
                if (IsRequestAuthenticated)
                {
                    var headerValues = _httpContext.Request.Headers[authorizationHeaderName];

                    if (headerValues.Count > 0)
                    {
                        var splited = headerValues[0].Split(" ", StringSplitOptions.RemoveEmptyEntries);

                        if (splited.Length == 2)
                            result = splited[1];
                    }
                }

                // override token value if value present in config
                if (!string.IsNullOrWhiteSpace(_authContextOverrides.EidAccessToken))
                    result = _authContextOverrides.EidAccessToken;

                return result;
            }
        }

        private bool IsRequestAuthenticated => _httpContext.User?.Identity?.IsAuthenticated ?? false;

        private EidAttribClaimValues GetAttributeValues()
        {
            EidAttribClaimValues result = EidAttribClaimValues.Empty();
            var attribClaim = GetUserClaim(attribClaimType);

            if (!string.IsNullOrEmpty(attribClaim))
            {
                try
                {
                    result = JsonConvert.DeserializeObject<EidAttribClaimValues>(attribClaim);
                }
                catch (Exception)
                {
                }
            }

            return result;
        }

        private string GetUserClaim(string claimType)
        {
            return IsRequestAuthenticated ?
                   _httpContext.User.FindFirstValue(claimType) :
                   null;
        }
    }
}

