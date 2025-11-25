using System;

namespace Eid.Microservices.MongoDb.Interface
{
    public interface IAuthContext
    {
        Guid PersonId { get; }
        Guid? TenantId { get; }
        string DisplayName { get; }
        string Email { get; }
        string Login { get; }

        bool IsAuthenticated { get; }
        string EIDAccessToken { get; }

        string RequestPath { get; }
        string QueryString { get; }
    }
}
