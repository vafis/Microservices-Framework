using System;
using System.Collections.Generic;
using System.Text;

namespace Eid.Microservices.IntegrationTests.Fixtures
{
    //[Parallelizable(ParallelScope.Fixtures)]
    public class MicroserviceFixture : IDisposable
    {
        public readonly MicroserviceContext MicroServiceTestContext = new MicroserviceContext();

        public MicroserviceFixture()
        {
            var app = new MicroserviceApplication();
            MicroServiceTestContext.Client = app.Client;
            MicroServiceTestContext.Server = app.Server;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
