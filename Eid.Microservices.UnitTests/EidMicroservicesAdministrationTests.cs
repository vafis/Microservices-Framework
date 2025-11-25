using System;
using System.Collections.Generic;
using System.Text;
using Eid.Microservices.Administration.Models;
using Eid.Microservices.Administration.Services;
using Eid.Microservices.Core.Options;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Eid.Microservices.UnitTests
{
    public class EidMicroservicesAdministrationTests
    {
        private IServiceFilterResolverService _serviceFilterResolverService;

        public EidMicroservicesAdministrationTests()
        {
            Initialize();
        }

        private void Initialize()
        {
            var applicationCoreOptionsMock = new Mock<IOptions<ApplicationCoreOptions>>();
            applicationCoreOptionsMock.Setup(x => x.Value)
                .Returns(new ApplicationCoreOptions() {ApplicationName = "MicroserviceBase" });
            _serviceFilterResolverService = new ServiceFilterResolverService(applicationCoreOptionsMock.Object);
        }

        [Fact]
        public void Can_ServiceFilter_Resolve()
        {
            var ret = _serviceFilterResolverService.RbacResolve();
            Assert.NotNull(ret);
            Assert.IsType<List<AssemblyModel>>(ret);
            Assert.True(ret.Count>1);
        }
    }
}
