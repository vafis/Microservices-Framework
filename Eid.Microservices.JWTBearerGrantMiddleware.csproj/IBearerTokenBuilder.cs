using System;
using System.Collections.Generic;
using System.Text;

namespace Eid.Microservices.EidJwtBearerGrantMiddleware
{
    public interface IBearerTokenBuilder
    {
        string BuildToken();
    }
}
