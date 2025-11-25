using System;
using System.Collections.Generic;
using System.Text;
using Eid.Microservices.Core.Engine;
using Eid.Microservices.Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Eid.Microservices.EngineContextAccessor
{
    public class EngineContextAccessor : IEngineContextAccessor
    {
        private static ITypeFinder _appDomainTypeFinder;
        private static IEngine _engineCurrent;
        
        public EngineContextAccessor(ITypeFinder appDomainTypeFinder)
        {
            _appDomainTypeFinder = appDomainTypeFinder;
            _engineCurrent =
                Singleton<IEngine>.Instance ?? (Singleton<IEngine>.Instance = new Engine(_appDomainTypeFinder));
        }
        public IEngine Engine
        {
            get
            {
                if (Singleton<IEngine>.Instance == null)
                    return _engineCurrent = Singleton<IEngine>.Instance ?? (Singleton<IEngine>.Instance = new Engine(_appDomainTypeFinder));
                return Singleton<IEngine>.Instance;
            }
            set => Engine = _engineCurrent;
        }
    }
}
