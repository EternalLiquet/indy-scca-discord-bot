using Serilog;

namespace indy_scca_discord_bot.Config
{
    public static class LoggingConfig
    {
        public static void CreateLoggerConfiguration()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            Log.Information("Logger Config complete");
        }
    }
}
