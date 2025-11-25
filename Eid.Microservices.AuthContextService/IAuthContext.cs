using System;
using System.Collections.Generic;
using System.Text;

namespace Eid.Microservices.AuthContextService
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
