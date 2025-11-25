using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Eid.Microservices.EidRbacAuthorizationServiceFilter
{
    public class HttpClientRbacAuthorization : IHttpClientRbacAuthorization
    {
        private const string RbacAuthorizationEndPoint = "/api/services/v1/extension/hasaccessToResource";
        private readonly HttpClient _httpClient;

        public HttpClientRbacAuthorization(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<HttpResponseMessage> RbackAuthorize(Dictionary<string, string> parameters)
        {
            var json = JsonConvert.SerializeObject(parameters);
            var request = new HttpRequestMessage(HttpMethod.Post, _httpClient.BaseAddress + RbacAuthorizationEndPoint)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            return await _httpClient.SendAsync(request);
        }
    }
}
