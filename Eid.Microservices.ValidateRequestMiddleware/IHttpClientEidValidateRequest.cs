using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Eid.Microservices.EidValidateRequestMiddleware
{
    public interface IHttpClientEidValidateRequest
    {
        Task<HttpResponseMessage> ValidateRequest(Dictionary<string, string> parameters);
    }
}
