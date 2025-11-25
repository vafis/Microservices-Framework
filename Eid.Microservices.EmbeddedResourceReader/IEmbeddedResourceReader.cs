using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Eid.Microservices.EmbeddedResourceReader
{
    public interface IEmbeddedResourceReader
    {
        X509Certificate2 GetCertificate(Type type, string resourceName, string password);
    }
}
