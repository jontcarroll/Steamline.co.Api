using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Steamline.co.Api.V1.Services;

namespace Steamline.co.Api.V1.Helpers
{
    public class ServiceActionResult : IActionResult
    {
        protected IActionResult _actionResult;


        public async Task ExecuteResultAsync(ActionContext context)
        {
            await _actionResult.ExecuteResultAsync(context);
        }
    }

    
}