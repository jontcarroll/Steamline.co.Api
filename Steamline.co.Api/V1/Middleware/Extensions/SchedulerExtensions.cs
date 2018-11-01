﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Steamline.co.Api.V1.Services;
using Steamline.co.Api.V1.Services.Interfaces;
using System;
using System.Threading.Tasks;

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
