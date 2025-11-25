using System.Security.Authentication;

namespace Eid.Microservices.MongoDb.Models
{
    public class MongoConnectionSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public bool UseSSL { get; set; }
        public SslProtocols SslProtocol { get; set; }
        public int? ConnectionTimeout { get; set; }
    }
}
