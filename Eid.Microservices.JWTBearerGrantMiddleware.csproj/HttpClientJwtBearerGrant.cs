using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Eid.Microservices.EidJwtBearerGrantMiddleware
{
    public class HttpClientJwtBearerGrant : IHttpClientJwtBearerGrant
    {
        private const string RequestTokenEndPoint = "/oauth/v2/token";
        private const string RefreshTokenEndPoint = "/oauth/v2/token";

        private readonly HttpClient _httpClient;

        public HttpClientJwtBearerGrant(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> RequestToken(Dictionary<string, string> parameters)
        {

            var request = new HttpRequestMessage(HttpMethod.Post, _httpClient.BaseAddress + RequestTokenEndPoint)
            {
                Content = new FormUrlEncodedContent(parameters)
            };

            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> RefreshToken(Dictionary<string, string> parameters)
        {

            var request = new HttpRequestMessage(HttpMethod.Post, _httpClient.BaseAddress + RefreshTokenEndPoint)
            {
                Content = new FormUrlEncodedContent(parameters)
            };

            return await _httpClient.SendAsync(request);
        }
    }
}
