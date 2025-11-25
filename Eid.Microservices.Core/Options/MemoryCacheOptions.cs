using System;
using System.Collections.Generic;
using System.Text;

namespace Eid.Microservices.Core.Options
{
    public class MemoryCacheOptions
    {
        public EidValidateRequestCacheOptions EidValidateRequestCacheOptions { get; set; }
        public RbacAuthorizationCacheOptions RbacAuthorizationCacheOptions { get; set; }
    }

    public class EidValidateRequestCacheOptions
    {
        public bool Enable { get; set; }
        public TimeSpan ExpirationTime { get; set; }
    }

    public class RbacAuthorizationCacheOptions
    {
        public bool Enable { get; set; }
        public TimeSpan ExpirationTime { get; set; }
    }
}
