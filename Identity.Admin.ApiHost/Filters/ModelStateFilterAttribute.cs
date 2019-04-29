using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Upo.Identity.Admin.ApiHost
{
    public class ModelStateFilterAttribute : ActionFilterAttribute
    {
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var modelState = context.ModelState;
            if (!modelState.IsValid)
            {
                var error = string.Join(", ", modelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage));
                throw new ArgumentException(error);
            }
            return base.OnActionExecutionAsync(context, next);
        }
    }
}
