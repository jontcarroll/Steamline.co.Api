using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Steamline.co.Api.V1.Services;

namespace Steamline.co.Api.V1.Middleware.Extensions
{
    public static class SchedulerExtensions
    {
        public static IServiceCollection AddScheduler(this IServiceCollection services)
        {
            return services.AddSingleton<IHostedService, SchedulerHostedService>();
        }
    }
}
