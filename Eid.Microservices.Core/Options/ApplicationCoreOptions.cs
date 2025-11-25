using System;
using System.Collections.Generic;
using System.Text;

namespace Eid.Microservices.Core.Options
{
    public class ApplicationCoreOptions
    {
        public string ApplicationName { get; set; }
        public bool EnableAuthentication { get; set; }
        public string EmbeddedResourceQualifier { get; set; }
    }
}
