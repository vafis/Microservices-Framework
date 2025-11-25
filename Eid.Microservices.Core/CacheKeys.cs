using System;
using System.Collections.Generic;
using System.Text;

namespace Eid.Microservices.Core
{
    public static class CacheKeys
    {
        public static string AccessTokenResponse => "_AccessTokenResponse";
        //public static string RefreshToken => "RefreshToken";
        public static string RemoteIpAddress => "_IP:";
        public static string RbacAuth => "_RbacAuth:";
        public static string SigningCertificate => "_SigningCertificate";
    }
}
