using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Coravel;
using Serilog;
using Serilog.Formatting.Json;
using dotenv.net;

namespace OrderProcessingWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotEnv.Load();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt", 
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                    rollingInterval: RollingInterval.Day)
                .WriteTo.File("logs/errorlog.txt", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning)
                .CreateLogger();

            try
            {
                Log.Information("Starting service..");

                var host = CreateHostBuilder(args).Build();
                host.Services.UseScheduler(scheduler =>
                {
                    var jobSchedule = scheduler.Schedule<ProcessOrder>();
                    jobSchedule
                        .EverySeconds(2)
                        .PreventOverlapping("ProcessOrderJob");
                });

                host.Run();
            }
            catch (System.Exception ex)
            {
                Log.Fatal(ex, "Exception in application");
            }
            finally
            {
                Log.Information("Exiting service");
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScheduler();
                    services.AddTransient<ProcessOrder>();
                });
        }
    }
}
