using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Eid.Microservices.Core.Http
{
    public class DefaultHttpClientHandler : HttpClientHandler
    {
        public DefaultHttpClientHandler() =>
            this.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
    }
}
