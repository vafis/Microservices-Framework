using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eid.Microservices.Administration.Models;
using Eid.Microservices.Administration.HttpClients;
using Eid.Microservices.Administration.Services;
using Eid.Microservices.EidRbacAuthorizationServiceFilter;
using Eid.Microservices.ValidateModelActionFilter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Eid.Microservices.BackChannelLoggerService;

namespace Eid.Microservices.Administration.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RbacAdminController : ControllerBase
    {
        private readonly IHttpClientAdmin _httpClientAdmin;
        private readonly IServiceFilterResolverService _serviceFilterResolverService;
        private readonly IBackChannelLoggerService<RbacAdminController> _backChannelLoggerService;


        public RbacAdminController(IHttpClientAdmin httpClientAdmin, 
                                   IServiceFilterResolverService serviceFilterResolverService,
                                   IBackChannelLoggerService<RbacAdminController> backChannelLoggerService)
        {
            _httpClientAdmin = httpClientAdmin;
            _serviceFilterResolverService = serviceFilterResolverService;
            _backChannelLoggerService = backChannelLoggerService;
        }


        [ServiceFilter(typeof(RbacAuthorizationServiceFilter))]
        [HttpPost("RegisterRbacAsync")]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ActionResult<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> RegisterRbacAsync([FromBody] List<AssemblyModel> assemblyModels)
        {
            
            return await await _httpClientAdmin.RbackRegisterAsync(assemblyModels)
                .ContinueWith(x =>
                {
                    if (x.IsFaulted)
                    {
                        if (x.Exception != null)
                            Task.Run(() => _backChannelLoggerService.LogError(x.Exception));
                        return Task.FromResult(false);
                    }
                    if (x.IsCanceled)
                    {
                        Task.Run(() => _backChannelLoggerService.LogError(new Exception("Current request was cancelled")));
                        return Task.FromResult(false);
                    }

                    return !x.Result.IsSuccessStatusCode ? Task.FromResult(false)
                        : Task.FromResult(JsonConvert.DeserializeObject<bool>(x.Result.Content.ReadAsStringAsync().Result));
                });

        }

        [HttpGet("ResolveRbac")]
        [ServiceFilter(typeof(RbacAuthorizationServiceFilter))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(List<AssemblyModel>), StatusCodes.Status200OK)]

        public List<AssemblyModel> ResolveRbac()
        {
            return _serviceFilterResolverService.RbacResolve();
        }
     
    }
}
