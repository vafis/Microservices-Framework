using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Eid.Microservices.Core.Options
{
    public class JwtBearerGrantOptions
    {
        public string ApiKey { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string CallbackUrl { get; set; }
        public string TokenAccessUrl { get; set; }
        public string AuthorizeUrl { get; set; }
        public string TokenInfoUrl { get; set; }
        public string UserInfoUrl { get; set; }
        public TimeSpan TokenExpirationTime { get; set; }
        public string Password { get; set; }


    }
}
