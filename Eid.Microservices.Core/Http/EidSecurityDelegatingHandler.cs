using Eid.Microservices.AuthContextService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Eid.Microservices.Core.Http
{
    public class EidSecurityDelegatingHandler : DelegatingHandler
    {
        private const string ApiKeyHeader = "X-EmpowerID-API-Key";
        private readonly IAuthContext _authContext;

        public EidSecurityDelegatingHandler(IAuthContext authContext) => _authContext = authContext;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, _authContext.EIDAccessToken);
            request.Headers.Add(ApiKeyHeader, "69f9f967-c5dd-4f9d-80cc-1d4d616994f8"); // TODO: get from config


            return base.SendAsync(request, cancellationToken);
        }
    }
}
