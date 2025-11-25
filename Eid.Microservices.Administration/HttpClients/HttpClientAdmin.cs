using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Eid.Microservices.Administration.Models;
using Newtonsoft.Json;

namespace Eid.Microservices.Administration.HttpClients
{
    public class HttpClientAdmin : IHttpClientAdmin
    {
        private const string Key = "data";
        private const string RbacAuthorizationEndPoint = "/api/services/v1/extension/register";
        private readonly HttpClient _httpClient;

        public HttpClientAdmin(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> RbackRegisterAsync(List<AssemblyModel> assemblyModels)
        {
            var values = new Dictionary<string, string>
            {
                {Key, JsonConvert.SerializeObject(assemblyModels) }
            };
            var json = JsonConvert.SerializeObject(values);

            var request = new HttpRequestMessage(HttpMethod.Post, _httpClient.BaseAddress + RbacAuthorizationEndPoint)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            return await _httpClient.SendAsync(request);
        }
    }
}
