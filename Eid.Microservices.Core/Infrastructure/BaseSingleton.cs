using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Eid.Microservices.Core.Infrastructure
{
    public class BaseSingleton
    {
        static BaseSingleton() { }
        public static IDictionary<Type, object> AllSingletons => new ConcurrentDictionary<Type, object>();
    }
}
