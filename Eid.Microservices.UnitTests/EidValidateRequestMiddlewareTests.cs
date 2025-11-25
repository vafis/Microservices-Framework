using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Eid.Microservices.BackChannelLoggerService;
using Eid.Microservices.Core.Options;
using Eid.Microservices.EidValidateRequestMiddleware;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using MemoryCacheOptions = Eid.Microservices.Core.Options.MemoryCacheOptions;
using Microsoft.AspNetCore.Http;
using Xunit;
using Eid.Microservices.Core;

namespace Eid.Microservices.UnitTests
{
    public class EidValidateRequestMiddlewareTests
    {
        public EidValidateRequestMiddlewareTests()
        {

        }

        [Fact]
        public async Task Can_Validate_Request_with_Cache_Enabled_true_and_Returns_TrueAsync()
        {
            var httpClientMock = new Mock<IHttpClientEidValidateRequest>();

            var httpResponsemessage = new HttpResponseMessage()
                {StatusCode = HttpStatusCode.OK, Content = new StringContent("true")};

            httpClientMock.Setup(x => x.ValidateRequest(It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(httpResponsemessage);
            //Kostas:
            //Moq does not support extenssion methods mocking
            //var cacheMock=new Mock<IMemoryCache>();
            //cacheMock.Setup(x => x.Set<string>(It.IsAny<string>(), It.IsAny<string>())).Returns("true");
            var memoryCacheOptionsMock = new Mock<IOptions<MemoryCacheOptions>>();
            var cacheOptions = new MemoryCacheOptions()
            {
                EidValidateRequestCacheOptions = new EidValidateRequestCacheOptions() {Enable = true},
                RbacAuthorizationCacheOptions = null
            };
            memoryCacheOptionsMock.Setup(x => x.Value).Returns(cacheOptions);

            IMemoryCache cache = new MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions() { });

            var backChannelLoggerMock =
                new Mock<IBackChannelLoggerService<EidValidateRequestMiddleware.EidValidateRequestMiddleware>>();

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            //var sut = new EidValidateRequestMiddleware.EidValidateRequestMiddleware((innerHttpContext) => { return innerHttpContext.Response.WriteAsync("test response body"); },
            //   httpClientMock.Object, cacheMock.Object, memoryCacheOptionsMock.Object, backChannelLoggerMock.Object);

            var nextDelegateCalled = false;
            var sut = new EidValidateRequestMiddleware.EidValidateRequestMiddleware(async (c) =>
                {
                    await Task.Delay(0);
                    nextDelegateCalled = true;
                },
                httpClientMock.Object, cache, memoryCacheOptionsMock.Object, backChannelLoggerMock.Object);
            await sut.InvokeAsync(context);
            Assert.True(nextDelegateCalled);

        }

        [Fact]
        public async Task Can_Validate_Request_with_Cache_Enabled_false_and_Returns_TrueAsync()
        {
            var httpClientMock = new Mock<IHttpClientEidValidateRequest>();

            var httpResponsemessage = new HttpResponseMessage()
                {StatusCode = HttpStatusCode.OK, Content = new StringContent("true")};

            httpClientMock.Setup(x => x.ValidateRequest(It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(httpResponsemessage);

            var memoryCacheOptionsMock = new Mock<IOptions<MemoryCacheOptions>>();
            var cacheOptions = new MemoryCacheOptions()
            {
                EidValidateRequestCacheOptions = new EidValidateRequestCacheOptions() {Enable = false},
                RbacAuthorizationCacheOptions = null
            };
            memoryCacheOptionsMock.Setup(x => x.Value).Returns(cacheOptions);

            IMemoryCache cache = new MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions() { });

            var backChannelLoggerMock =
                new Mock<IBackChannelLoggerService<EidValidateRequestMiddleware.EidValidateRequestMiddleware>>();

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var nextDelegateCalled = false;
            var sut = new EidValidateRequestMiddleware.EidValidateRequestMiddleware(async (c) =>
                {
                    await Task.Delay(0);
                    nextDelegateCalled = true;
                },
                httpClientMock.Object, cache, memoryCacheOptionsMock.Object, backChannelLoggerMock.Object);
            await sut.InvokeAsync(context);
            Assert.True(nextDelegateCalled);

        }

        [Fact]
        public async Task Can_Validate_Request_with_Cache_Enabled_true_and_IP_cache_exists_and_Returns_TrueAsync()
        {
            var httpClientMock = new Mock<IHttpClientEidValidateRequest>();

            var httpResponsemessage = new HttpResponseMessage()
                {StatusCode = HttpStatusCode.OK, Content = new StringContent("true")};

            httpClientMock.Setup(x => x.ValidateRequest(It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(httpResponsemessage);

            var memoryCacheOptionsMock = new Mock<IOptions<MemoryCacheOptions>>();
            var cacheOptions = new MemoryCacheOptions()
            {
                EidValidateRequestCacheOptions = new EidValidateRequestCacheOptions() {Enable = true},
                RbacAuthorizationCacheOptions = null
            };
            memoryCacheOptionsMock.Setup(x => x.Value).Returns(cacheOptions);

            IMemoryCache cache = new MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions() { });
            var cacheExpirationOptions = new MemoryCacheEntryOptions()
            {
                Priority = CacheItemPriority.Normal,
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(1)
            };
            string key = CacheKeys.RemoteIpAddress + "127.0.0.1";
            cache.Set(key, "true", cacheExpirationOptions);

            var backChannelLoggerMock =
                new Mock<IBackChannelLoggerService<EidValidateRequestMiddleware.EidValidateRequestMiddleware>>();

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var nextDelegateCalled = false;
            var sut = new EidValidateRequestMiddleware.EidValidateRequestMiddleware(async (c) =>
                {
                    await Task.Delay(0);
                    nextDelegateCalled = true;
                },
                httpClientMock.Object, cache, memoryCacheOptionsMock.Object, backChannelLoggerMock.Object);
            await sut.InvokeAsync(context);
            Assert.True(nextDelegateCalled);
        }

        [Fact]
        public async Task Can_Validate_Request_with_Cache_Enabled_true_and_IP_cache_exists_and_Returns_falseAsync()
        {
            var httpClientMock = new Mock<IHttpClientEidValidateRequest>();

            var httpResponsemessage = new HttpResponseMessage()
                {StatusCode = HttpStatusCode.OK, Content = new StringContent("False")};

            httpClientMock.Setup(x => x.ValidateRequest(It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(httpResponsemessage);

            var memoryCacheOptionsMock = new Mock<IOptions<MemoryCacheOptions>>();
            var cacheOptions = new MemoryCacheOptions()
            {
                EidValidateRequestCacheOptions = new EidValidateRequestCacheOptions() {Enable = true},
                RbacAuthorizationCacheOptions = null
            };
            memoryCacheOptionsMock.Setup(x => x.Value).Returns(cacheOptions);

            IMemoryCache cache = new MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions() { });
            var cacheExpirationOptions = new MemoryCacheEntryOptions()
            {
                Priority = CacheItemPriority.Normal,
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(1)
            };
            string key = CacheKeys.RemoteIpAddress + "127.0.0.1";
            cache.Set(key, "false", cacheExpirationOptions);

            var backChannelLoggerMock =
                new Mock<IBackChannelLoggerService<EidValidateRequestMiddleware.EidValidateRequestMiddleware>>();

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var nextDelegateCalled = false;
            var sut = new EidValidateRequestMiddleware.EidValidateRequestMiddleware(async (c) =>
                {
                    await Task.Delay(0);
                    nextDelegateCalled = true;
                },
                httpClientMock.Object, cache, memoryCacheOptionsMock.Object, backChannelLoggerMock.Object);
            await sut.InvokeAsync(context);
            Assert.False(nextDelegateCalled);
        }

        [Fact]
        public async Task Can_Validate_Request_with_Cache_Enabled_false_and__Returns_falseAsync()
        {
            var httpClientMock = new Mock<IHttpClientEidValidateRequest>();

            var httpResponsemessage = new HttpResponseMessage()
            { StatusCode = HttpStatusCode.OK, Content = new StringContent("false") };

            httpClientMock.Setup(x => x.ValidateRequest(It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(httpResponsemessage);

            var memoryCacheOptionsMock = new Mock<IOptions<MemoryCacheOptions>>();
            var cacheOptions = new MemoryCacheOptions()
            {
                EidValidateRequestCacheOptions = new EidValidateRequestCacheOptions() { Enable = false },
                RbacAuthorizationCacheOptions = null
            };
            memoryCacheOptionsMock.Setup(x => x.Value).Returns(cacheOptions);

            IMemoryCache cache = new MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions() { });

            var backChannelLoggerMock =
                new Mock<IBackChannelLoggerService<EidValidateRequestMiddleware.EidValidateRequestMiddleware>>();

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var nextDelegateCalled = false;
            var sut = new EidValidateRequestMiddleware.EidValidateRequestMiddleware(async (c) =>
            {
                await Task.Delay(0);
                nextDelegateCalled = true;
            },
                httpClientMock.Object, cache, memoryCacheOptionsMock.Object, backChannelLoggerMock.Object);
            await sut.InvokeAsync(context);
            Assert.False(nextDelegateCalled);
        }
    }

}
