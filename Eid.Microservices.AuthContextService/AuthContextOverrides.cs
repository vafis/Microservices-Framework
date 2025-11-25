using System;
using System.Collections.Generic;
using System.Text;

namespace Eid.Microservices.AuthContextService
{
    public class AuthContextOverrides
    {
        public string PersonId { get; set; }
        public string TenantId { get; set; }
        public string IsAuthenticated { get; set; }
        public string EidAccessToken { get; set; }
    }
}
