using System;
using System.Collections.Generic;
using System.Text;
using Eid.Microservices.Core.Engine;

namespace Eid.Microservices.EngineContextAccessor
{
    public interface IEngineContextAccessor
    {
        IEngine Engine { get; set; }
    }
}
