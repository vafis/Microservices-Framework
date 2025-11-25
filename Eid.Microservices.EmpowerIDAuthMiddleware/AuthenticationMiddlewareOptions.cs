using System.Collections.Generic;
using System.Diagnostics;

namespace Eid.Microservices.AuthentiicationMiddleware
{
    public class AuthenticationMiddlewareOptions
    {
        public string Issuer { get; set; }
        public List<string> ValidAudiences { get; set; }
        public string MetadataAddress { get; set; }
    }
}
