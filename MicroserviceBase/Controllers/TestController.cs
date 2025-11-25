using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eid.Microservices.AuthentiicationMiddleware;
using Eid.Microservices.Core.Security;
using Eid.Microservices.EidRbacAuthorizationServiceFilter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MicroserviceBase.Controllers
{
    //[ServiceFilter(typeof(RbacAuthorizationServiceFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        
        public TestController()
        {
            
        }

        //[Authorize(AuthenticationSchemes = AnonymousAuthOptions.DefaultScheme)]
        [Authorize(Policy = nameof(Policy.AllowAnonymous))]
        [HttpGet("basic-authz")]
        public IActionResult BasicAuthorization()
        {
            return Ok();
        }



        [HttpGet("RbacCheck")]
        [ServiceFilter(typeof(RbacAuthorizationServiceFilter))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult RbacCheck()
        {
            return Ok();
        }

        [HttpGet("AllowAnonymousCheck1")]
        [Authorize(Policy = nameof(Policy.AllowAnonymous))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult AllowAnonymousCheck1()
        {
            return Ok();
        }

        [HttpGet("AllowAnonymousCheck2")]
        [Authorize(Policy = nameof(Policy.AllowAnonymous))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult AllowAnonymousCheck2()
        {
            return Ok();
        }
    }

}