using Serilog;
using Serilog.Events;

namespace Microservices.CAS.Configs;

public static class LogConfig
{
    public static void AddLog(this IHostBuilder host)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Warning()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        host.UseSerilog();
    }
}
