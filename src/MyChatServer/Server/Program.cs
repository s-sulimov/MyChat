using Serilog;
using Serilog.Formatting.Compact;
using System.Globalization;

namespace Sulimov.MyChat.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(formatProvider: new CultureInfo("en-US"))
                .CreateLogger();

            try
            {
                Log.Information("Application is starting...");

                CreateHostBuilder(args).UseSerilog().Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }

            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

    }
}