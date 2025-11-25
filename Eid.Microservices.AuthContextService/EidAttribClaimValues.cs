using System;
using System.Collections.Generic;
using System.Text;

namespace Eid.Microservices.AuthContextService
{
    public class EidAttribClaimValues
    {
        public Guid PersonGuid { get; set; }
        public Guid TenantGuid { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }

        internal static EidAttribClaimValues Empty()
        {
            return new EidAttribClaimValues
            {
                PersonGuid = Guid.Empty,
                TenantGuid = Guid.Empty,
                DisplayName = string.Empty,
                Email = string.Empty
            };
        }

    }
}
