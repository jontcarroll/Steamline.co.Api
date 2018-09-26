using System;
using System.Linq;
using System.Threading.Tasks;   

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Antiforgery;


namespace Steamline.co.Api.V1.Middleware
{
    public class XSRFCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public XSRFCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAntiforgery antiforgery)
        {
            var result = antiforgery.ValidateRequestAsync(context);

            if (result.IsFaulted) {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                // end request here
                return;
            }
            else {
                // continue on the pipeline
                await _next(context);
            }
        }
    }
}