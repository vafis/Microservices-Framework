using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Eid.Microservices.ValidateModelActionFilter
{
    public class ValidateModel: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid || context.ModelState == null)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }
    }
}
