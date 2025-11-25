using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using MicroserviceBase;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace Eid.Microservices.IntegrationTests
{
    public class MicroserviceApplication
    {
        public MicroserviceApplication()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    hostingContext.HostingEnvironment.EnvironmentName = "Development";

                });

            Server = new TestServer(builder);
            Client = Server.CreateClient();
            //Client.BaseAddress = new Uri("http://localhost:5012");
        }

        public TestServer Server { get; }
        public HttpClient Client { get; }
        public IConfiguration Configuration { get; set; }
    }
}
