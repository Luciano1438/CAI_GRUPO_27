using Serilog;
using Serilog.Events;
using Serilog.Filters;

namespace Products.API.Extensions;

public static class LoggingExtensions
{
    public static void AddAppLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Logger(logger => logger
                .Filter.ByIncludingOnly(logEvent =>
                {
                    var isRequestLog = Matching.FromSource("Serilog.AspNetCore.RequestLoggingMiddleware")(logEvent);
                    if (!isRequestLog)
                    {
                        return false;
                    }

                    if (logEvent.Properties.TryGetValue("RequestPath", out var pathValue) &&
                        pathValue is ScalarValue scalar &&
                        scalar.Value is string path)
                    {
                        return !path.Contains("/health", StringComparison.OrdinalIgnoreCase) &&
                               !path.Contains("/swagger", StringComparison.OrdinalIgnoreCase);
                    }

                    return true;
                })
                .WriteTo.File(
                    path: "logs/audit.log",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} | {RequestMethod} | {RequestPath} | {StatusCode}{NewLine}",
                    rollingInterval: RollingInterval.Day))
            .CreateLogger();

        builder.Host.UseSerilog();
    }
}
