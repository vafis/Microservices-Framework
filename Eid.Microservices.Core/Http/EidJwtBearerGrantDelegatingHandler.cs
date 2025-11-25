using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eid.Microservices.Core.Options;
using Microsoft.Extensions.Options;

namespace Eid.Microservices.Core.Http
{
    public class EidJwtBearerGrantDelegatingHandler : DelegatingHandler
    {
        private const string ApiKeyHeader = "X-EmpowerID-API-Key";

        private readonly JwtBearerGrantOptions _jwtBearerGrantOptions;

        public EidJwtBearerGrantDelegatingHandler(IOptions<JwtBearerGrantOptions> jwtBearerGrantOptions)
        {
            _jwtBearerGrantOptions = jwtBearerGrantOptions.Value;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add(ApiKeyHeader, _jwtBearerGrantOptions.ApiKey);

            return base.SendAsync(request, cancellationToken);
        }





    }
}
