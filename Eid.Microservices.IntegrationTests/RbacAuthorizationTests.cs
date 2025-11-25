using Eid.Microservices.IntegrationTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Eid.Microservices.IntegrationTests
{
    public class RbacAuthorizationTests : IClassFixture<MicroserviceFixture>
    {
        private MicroserviceFixture _microserviceFixture;

        public RbacAuthorizationTests(MicroserviceFixture microserviceFixture)
        {
            _microserviceFixture = microserviceFixture;
        }

        [Fact]
        public async Task RbacAuthorizationCheck()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/test/RbacCheck");
            request.Headers.Authorization =
                new AuthenticationHeaderValue("bearer", _microserviceFixture.MicroServiceTestContext.JWT);
            var response = await _microserviceFixture.MicroServiceTestContext.Client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
           
        }


    }
}
