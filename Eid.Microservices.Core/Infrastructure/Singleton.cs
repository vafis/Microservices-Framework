using System;
using System.Collections.Generic;
using System.Text;

namespace Eid.Microservices.Core.Infrastructure
{
    public class Singleton<T> : BaseSingleton
    {
        private static T _instance;

        public static T Instance
        {
            get => _instance;
            set
            {
                _instance = value;
                AllSingletons[typeof(T)] = value;
            }
        }
    }
}
