using System;
using System.Collections.Generic;
using System.Text;
using Eid.Microservices.Core.Infrastructure;

namespace Eid.Microservices.StartupTaskExample
{
    public class StartUpExample : IStartupTask
    {
        public int Order => 100;

        public void Execute()
        {
            string test= "HELLO WORLD";
        }
    }
}
