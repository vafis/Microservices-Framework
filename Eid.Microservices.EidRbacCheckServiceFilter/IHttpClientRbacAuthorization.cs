using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Eid.Microservices.EidRbacAuthorizationServiceFilter
{
    public interface IHttpClientRbacAuthorization
    {
        Task<HttpResponseMessage> RbackAuthorize(Dictionary<string, string> parameters);
    }
}
