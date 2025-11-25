using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication;

namespace Eid.Microservices.Core.Security
{
    public class AnonymousAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "Anonymous_Auth";
        public const string DefaultSid = "0f8fad5b-d9cb-469f-a165-70867728950e";
        public string Scheme => DefaultScheme;

        public string Sid => DefaultSid;
    }
}
