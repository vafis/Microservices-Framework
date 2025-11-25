using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Eid.Microservices.Administration.Models;
using Eid.Microservices.IntegrationTests.Fixtures;
using Newtonsoft.Json;
using Xunit;

namespace Eid.Microservices.IntegrationTests
{
    public class EidMicroservicesAdministrationTests : IClassFixture<MicroserviceFixture>
    {
        private MicroserviceFixture _microserviceFixture;

        public EidMicroservicesAdministrationTests(MicroserviceFixture microserviceFixture)
        {
            _microserviceFixture = microserviceFixture;
        }

        [Fact]
        public async Task Can_RegisterRbacAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/RbacAdmin/RegisterRbacAsync");
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", _microserviceFixture.MicroServiceTestContext.JWT);
            var assemblyModels = new List<AssemblyModel>();
            // assemblyModels.Add(new AssemblyModel(){Parent = "MicroserviceBase", Members = new List<string>(){ "TestController.RbacCheck" } });
            assemblyModels.Add(new AssemblyModel() { Parent = "MicroserviceBase", Members = new List<string>() { "RbacAdminController.RegisterRbacAsync" } });
            var json = JsonConvert.SerializeObject(assemblyModels);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _microserviceFixture.MicroServiceTestContext.Client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(bool.Parse(response.Content.ReadAsStringAsync().Result));
        }

        [Fact]
        public async Task Can_ResolveRbacAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/RbacAdmin/ResolveRbac");
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", _microserviceFixture.MicroServiceTestContext.JWT);
            var response = await _microserviceFixture.MicroServiceTestContext.Client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var ret = response.Content.ReadAsStringAsync().Result;
            Assert.NotNull(ret);
            Assert.IsType<List<AssemblyModel>>(JsonConvert.DeserializeObject<List<AssemblyModel>>(ret));

        }
    }
}
