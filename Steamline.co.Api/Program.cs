using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Steamline.co.Api.V1.Helpers;
using System;
using System.IO;
using System.Linq;

namespace Steamline.co.Api
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();


        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .WriteTo.Logger(cc => cc.Filter.ByIncludingOnly(WithProperty("EventId", (int)LogEventId.General)).WriteTo.File(LogFilePath("SteamLineLog.txt"), rollingInterval: RollingInterval.Day))
                .WriteTo.Logger(cc => cc.Filter.ByIncludingOnly(WithProperty("EventId", (int)LogEventId.User)).WriteTo.File(LogFilePath("SteamLineUser.txt"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: null, rollOnFileSizeLimit: true))
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                Log.Information("Dynamic builder starting up...");

                BuildWebHost(args).Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static string LogFilePath(string name)
        {
            return Path.Combine(Environment.CurrentDirectory, "bin", name);
        }

        public static Func<LogEvent, bool> WithProperty(string propertyName, object scalarValue)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            var scalar = new ScalarValue(scalarValue);
            return e =>
            {
                if (e.Properties.TryGetValue(propertyName, out var propertyValue))
                {
                    if (propertyValue is StructureValue stValue)
                    {
                        var value = stValue.Properties.Where(cc => cc.Name == "Id").FirstOrDefault();
                        bool result = scalar.Equals(value.Value);
                        return result;
                    }
                }
                return false;
            };
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseConfiguration(Configuration)
                .UseSerilog()
                .Build();
    }
}
