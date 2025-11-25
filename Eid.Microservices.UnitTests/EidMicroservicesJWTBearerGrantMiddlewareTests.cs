
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Eid.Microservices.BackChannelLoggerService;
using Eid.Microservices.Core;
using Eid.Microservices.Core.Options;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Eid.Microservices.EmbeddedResourceReader;
using Eid.Microservices.EidJwtBearerGrantMiddleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Eid.Microservices.UnitTests
{
    public class EidMicroservicesJwtBearerGrantMiddlewareTests
    {
        private const string EmbeddedResourceName = "Certificate.pfx";

        private readonly Mock<IOptions<JwtBearerGrantOptions>> _jwtBearerGrantOptionsMock = new Mock<IOptions<JwtBearerGrantOptions>>();
        private IMemoryCache _cache;
        private Mock<IHttpClientJwtBearerGrant> _httpClientJwtBearerGrantMock=new Mock<IHttpClientJwtBearerGrant>();
        Mock<IBackChannelLoggerService<EidJwtBearerGrantMiddleware.EidJwtBearerGrantMiddleware>> _backChannelLoggerMock =
            new Mock<IBackChannelLoggerService<EidJwtBearerGrantMiddleware.EidJwtBearerGrantMiddleware>>();
        Mock<IBearerTokenBuilder> _tokenBuilderMock=new Mock<IBearerTokenBuilder>();
        Mock<ILogger<BearerTokenBuilder>> _loggerMock = new Mock<ILogger<BearerTokenBuilder>>();
        private readonly Mock<IOptions<ApplicationCoreOptions>> _applicationCoreOptionsMock = new Mock<IOptions<ApplicationCoreOptions>>();
        private readonly EmbeddedResourceReader.EmbeddedResourceReader _embeddedResourceReader=new EmbeddedResourceReader.EmbeddedResourceReader();

        public EidMicroservicesJwtBearerGrantMiddlewareTests()
        {
            _cache = new MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions() { });
             InitializeSetup();
        }

        private void InitializeSetup()
        {
            _jwtBearerGrantOptionsMock.Setup(x => x.Value).Returns(new JwtBearerGrantOptions()
            {
                TokenExpirationTime = new TimeSpan(10, 0, 0, 0),
                ClientId = "xxxx",
                CallbackUrl = "https://yourserver/WebIdPForms/OAuth/v2",
                Secret = "11a7ecf3-a94d-4ec5-91da-dff2e4a98ef3",
                AuthorizeUrl = "https://xxx/OAuth/v2/ui/authorize",
                TokenAccessUrl = "https://xxx/OAuth/v2/token",
                TokenInfoUrl = "https://xxx/OAuth/v2/tokeninfo",
                UserInfoUrl = "https://xxx/OAuth/v2/userinfo",
                Password = "xxx"
            });

            _applicationCoreOptionsMock.Setup(x => x.Value).Returns(new ApplicationCoreOptions()
            {
                ApplicationName = "MicroserviceBase",
                EnableAuthentication = true,
                EmbeddedResourceQualifier = "MicroserviceBase"
            });

            var accessTokenResponse = new AccessTokenResponse()
            {
                RefreshToken = "refreshToken",
                AccessToken = "accessToken",
                ExpiresIn = 100,
                TokenType = "Bearer",
                Id = "2323sq"
               // IdToken = "sdsdsd"
            };
            var httpResponsemessage = new HttpResponseMessage()
                { StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonConvert.SerializeObject(accessTokenResponse))};
            _httpClientJwtBearerGrantMock.Setup(x => x.RequestToken(It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(httpResponsemessage);

            _tokenBuilderMock.Setup(x => x.BuildToken()).Returns(
                "eyJhbGciOiJSUzI1NiIsImtpZCI6IkZrVzRuNG53bGwxV1RNOWZlcGNUc0tYUzZfSSIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwc");
        }

        [Fact]
        public async Task Can_Recieve_a_token_from_EmpowerId_with_no_exists_in_Cache_and_Next()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            
            var nextDelegateCalled = false;
            var sut = new EidJwtBearerGrantMiddleware.EidJwtBearerGrantMiddleware(async (c) =>
                {
                    await Task.Delay(0);
                    nextDelegateCalled = true;
                }, 
                _httpClientJwtBearerGrantMock.Object, _jwtBearerGrantOptionsMock.Object, _cache, 
                _backChannelLoggerMock.Object, _tokenBuilderMock.Object
                );
            await sut.InvokeAsync(context);
            Assert.True(nextDelegateCalled);
        }


        [Fact]
        public void BearerTokenBuilder_Can_Build_Token_without_SigningCertificate_in_cache()
        {
           var sut=new BearerTokenBuilder(_jwtBearerGrantOptionsMock.Object, _embeddedResourceReader,
               _loggerMock.Object, _applicationCoreOptionsMock.Object, _cache);

           var ret = sut.BuildToken();
           Assert.NotNull(ret);
           Assert.IsType<string>(ret);

        }

        [Fact]
        public void BearerTokenBuilder_Can_Build_Token_with_SigningCertificate_in_cache()
        {
            var asm = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(x => x.FullName.Contains(_applicationCoreOptionsMock.Object.Value.EmbeddedResourceQualifier));
            var entryName = asm.GetName().Name;
            var resourceName = $"{entryName}.{EmbeddedResourceName}";

            var signingCertificate = _embeddedResourceReader.GetCertificate(asm.GetExportedTypes().FirstOrDefault(), resourceName,
                _jwtBearerGrantOptionsMock.Object.Value.Password);
            _cache.Set(CacheKeys.SigningCertificate, signingCertificate);

            var sut = new BearerTokenBuilder(_jwtBearerGrantOptionsMock.Object, _embeddedResourceReader,
                _loggerMock.Object, _applicationCoreOptionsMock.Object, _cache);

            var ret = sut.BuildToken();
            Assert.NotNull(ret);
            Assert.IsType<string>(ret);

        }

    }
}
