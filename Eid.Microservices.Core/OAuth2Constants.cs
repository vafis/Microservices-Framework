using System;
using System.Collections.Generic;
using System.Text;

namespace Eid.Microservices.Core
{
    public static class OAuth2Constants
    {
        public const string GrantType = "grant_type";
        public const string ClientId = "client_id";
        public const string Secret = "client_secret";
        public const string RefreshToken = "refresh_token";
        public const string Scope = "scope";
        public const string Password = "password";
        public const string Assertion = "assertion";

        public static class GrantTypes
        {
            public const string Password = "password";
            public const string RefreshToken = "refresh_token";
            public const string JWTBearerGrant = "urn:ietf:params:oauth:grant-type:jwt-bearer";
        }
    }
}
