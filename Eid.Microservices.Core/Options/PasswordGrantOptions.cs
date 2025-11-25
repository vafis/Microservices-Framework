using System;
using System.Collections.Generic;
using System.Text;

namespace Eid.Microservices.Core.Options
{
    public class PasswordGrantOptions
    {
        public string Basic { get; set; }
        public string ApiKey { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
        //public string GrantType { get; set; }
    }
}
