using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Eid.Microservices.EidValidateRequestMiddleware
{
    public class HttpClientEidValidateRequest :IHttpClientEidValidateRequest
    {
        private const string ValidateRequestEndPoint = "/api/services/v1/extension/isvalidrequest";

        private readonly HttpClient _httpClient;

        public HttpClientEidValidateRequest(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> ValidateRequest(Dictionary<string,string> parameters)
        {
            var json = JsonConvert.SerializeObject(parameters);
            var request = new HttpRequestMessage(HttpMethod.Post, _httpClient.BaseAddress + ValidateRequestEndPoint)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            return await _httpClient.SendAsync(request);
        }
    }
}
