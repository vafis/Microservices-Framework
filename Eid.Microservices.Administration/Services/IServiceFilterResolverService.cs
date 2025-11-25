using System;
using System.Collections.Generic;
using System.Text;
using Eid.Microservices.Administration.Models;

namespace Eid.Microservices.Administration.Services
{
    public interface IServiceFilterResolverService
    {
        List<AssemblyModel> RbacResolve();
    }
}
