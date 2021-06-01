using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ordering.API.Extensions;
using Ordering.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;

namespace Ordering.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureLogger();
            Log.Information("Application started");
            try {
                CreateHostBuilder(args)
             .Build()
             .MigrateDatabase<OrderContext>((context, services) =>
             {
                 var logger = services.GetService<ILogger<OrderContextSeed>>();
                 OrderContextSeed
                     .SeedAsync(context, logger)
                     .Wait();
             })
             .Run();
            }
            catch(Exception ex)
            {
                Log.Error(ex.Message);
            }
            finally
            {
                Log.CloseAndFlush();
            }
          
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseSerilog();
                });


        private static void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
                         .WriteTo.Console()
                         .WriteTo.File(@"log.txt", Serilog.Events.LogEventLevel.Verbose,"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                         .CreateLogger();
        }
    }
}
