using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Eid.Microservices.Administration.Models;

namespace Eid.Microservices.Administration.HttpClients
{
    public interface IHttpClientAdmin
    {
        Task<HttpResponseMessage> RbackRegisterAsync(List<AssemblyModel> parameters);
    }
}
