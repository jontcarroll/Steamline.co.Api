using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Steamline.co.Api.V1.Middleware.Extensions
{
    public static class WebSocketExtensions
    {
        public static IApplicationBuilder UseCustomWebSocketManager(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CustomWebSocketManager>();
        }
    }
}
