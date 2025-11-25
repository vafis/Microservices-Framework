using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eid.Microservices.Core.Security
{
    public class AllowAnonymousRequirement : IAuthorizationRequirement
    {
        public string Sid { get; }

        public AllowAnonymousRequirement(string sid)
        {
            Sid= sid ?? throw new ArgumentNullException(nameof(sid));
        }


    }
}
