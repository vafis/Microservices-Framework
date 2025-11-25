using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace Eid.Microservices.IntegrationTests.Fixtures
{
    public class MicroserviceContext
    {
        public MicroserviceContext()
        {
            var config = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json").Build();
            this.JWT = config["JWT"];
            this.ComponentConfigSettings = config;
        }

        public IConfigurationRoot ComponentConfigSettings { get; set; }
        public TestServer Server { get; set; }
        public HttpClient Client { get; set; }
        public string JWT { get; set; }
    }
}
