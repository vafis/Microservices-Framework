using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Eid.Microservices.EidRequestTokenMiddleware
{
    public interface IHttpClientRequestToken
    {
        Task<HttpResponseMessage> RequestToken(Dictionary<string, string> parameters);
        Task<HttpResponseMessage> RefreshToken(Dictionary<string, string> parameters);
    }
}
