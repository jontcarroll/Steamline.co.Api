using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Steamline.co.Api.V1.Services.Interfaces;
using Steamline.co.Api.V1.Services;
using Steamline.co.Api.V1.Services.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Steamline.co.Api.V1.Middleware.Extensions;
using Steamline.co.Api.V1.Filters;
using Steamline.co.Api.V1.Services.Utils;
using Steamline.co.Api.V1.Config;
using System.Net.WebSockets;
using System.Threading;
using Steamline.co.Api.V1.Services.Websocket;
using Steamline.co.Api.V1.Middleware;
using Steamline.co.Api.V1.Services.ScheduledTasks;

namespace Steamline.co.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => {
                options.Filters.Add(typeof(ModelValidationActionFilter));
            });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin();
                        builder.AllowAnyMethod();
                        builder.AllowAnyHeader();
                        builder.AllowCredentials();
                    });
            });


            services.AddApiVersioning();

            services.AddHttpContextAccessor();

            // Config
            services.Configure<SteamApiConfig>(Configuration.GetSection("SteamApiConfig"));
            services.Configure<ElasticSearchConfig>(Configuration.GetSection("ElasticSearch"));

            // New every time it is injected
            services.AddTransient<IGameFinderService, GameFinderService>();
            services.AddTransient<ISteamService, SteamService>();
            services.AddTransient<ElasticService>();
            services.AddTransient<GameSearchService>();

            // New per request .AddScoped<>(...)

            // Only created when server starts
            services.AddSingleton<IWorkerQueue, WorkerQueue>();
  

            // Hosted Services (Can be used for generating group IDs to prevent issues with back to back code generation)
            services.AddHostedService<WorkerQueueHostedService>();

            // Add scheduled tasks & scheduler
            services.AddSingleton<IScheduledTask, AddMissingApps>();
            services.AddSingleton<IScheduledTask, UpdateAppDetails>();
            services.AddScheduler();

            services.AddSingleton<ICustomWebSocketFactory, CustomWebSocketFactory>();
            services.AddSingleton<ICustomWebSocketMessageHandler, CustomWebSocketMessageHandler>();

            services.AddAuthorization(options => {
                options.AddPolicy("SignedIn", p => {
                    p.RequireClaim("signedin");
                });
            });

            var signingKeyBase64 = Configuration.GetValue<string>("JwtOptions:SigningKey");
            var key = new SymmetricSecurityKey(Convert.FromBase64String(signingKeyBase64));
            services.AddAuthentication(options => {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters() {
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            }); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowAllOrigins");
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseWebSockets();
            app.UseCustomWebSocketManager();
            app.UseMvc();
        }
    }
}
