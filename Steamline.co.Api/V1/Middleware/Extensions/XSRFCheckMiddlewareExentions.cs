using Microsoft.AspNetCore.Builder;

using Steamline.co.Api.V1.Middleware;

namespace Steamline.co.Api.V1.Middleware.Extensions
{
    public static class XSRFCheckMiddlewareExtensions
    {
        public static IApplicationBuilder UseXSRFCheck(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<XSRFCheckMiddleware>();
        }
    } 
}

