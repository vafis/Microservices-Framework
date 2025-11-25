using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eid.Microservices.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MicroserviceBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Test2Controller : ControllerBase
    {
        [HttpGet("AllowAnonymousCheck")]
        [Authorize(Policy = nameof(Policy.AllowAnonymous))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult AllowAnonymousCheck()
        {
            return Ok();
        }
    }
}