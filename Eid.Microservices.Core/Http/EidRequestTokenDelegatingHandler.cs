using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eid.Microservices.Core.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace Eid.Microservices.Core.Http
{
    public class EidRequestTokenDelegatingHandler : DelegatingHandler
    {
        private const string ApiKeyHeader = "X-EmpowerID-API-Key";
        private readonly PasswordGrantOptions _requestTokenOptions;
        
        public EidRequestTokenDelegatingHandler(IOptions<PasswordGrantOptions> requestTokenOptions)
        {
            _requestTokenOptions = requestTokenOptions.Value;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _requestTokenOptions.Basic);
            request.Headers.Add(ApiKeyHeader, _requestTokenOptions.ApiKey); 

            return base.SendAsync(request, cancellationToken);
        }
    }
}
