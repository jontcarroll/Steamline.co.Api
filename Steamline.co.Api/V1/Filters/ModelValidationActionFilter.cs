using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

using Steamline.co.Api.V1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Steamline.co.Api.V1.Filters
{
    public class ModelValidationActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ModelState.IsValid) {
                var resultContext = await next();
            } 

            var error = new ValidationErrorModel();

            foreach (var key in context.ModelState.Keys) {
                var state = context.ModelState[key];

                if (state.ValidationState != ModelValidationState.Invalid) {
                    continue;
                }

                var errors = state.Errors.Select(p => p.ErrorMessage).ToList();
                error.Errors.AddRange(errors);

                if (!error.FieldErrors.ContainsKey(key)) {
                    error.FieldErrors.Add(key, errors);
                }
            }

            context.Result = new BadRequestObjectResult(error);
        }
    }
}