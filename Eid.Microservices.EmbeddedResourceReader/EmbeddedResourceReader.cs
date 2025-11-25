using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Eid.Microservices.EmbeddedResourceReader
{
    public class EmbeddedResourceReader : IEmbeddedResourceReader
    {
        public X509Certificate2 GetCertificate(Type type, string resourceName, string password)
        {
            
            using (var certificateStream = type.Assembly.GetManifestResourceStream(resourceName))
            {
                if (certificateStream == null)
                {
                    return null;
                }

                var rawBytes = new byte[certificateStream.Length];
                for (var i = 0; i < certificateStream.Length; i++)
                {
                    rawBytes[i] = (byte)certificateStream.ReadByte();
                }

                return new X509Certificate2(rawBytes, password, X509KeyStorageFlags.UserKeySet);
            }
        }
    }
}
