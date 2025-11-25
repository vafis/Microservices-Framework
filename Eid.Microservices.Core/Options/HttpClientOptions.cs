using System;
using System.Collections.Generic;
using System.Text;

namespace Eid.Microservices.Core
{
    public class HttpClientOptions
    {
        public Uri BaseAddress { get; set; }
        public TimeSpan TimeOut { get; set; }
        public string DefaultRequestHeadersAccept { get; set; }
    }
}
