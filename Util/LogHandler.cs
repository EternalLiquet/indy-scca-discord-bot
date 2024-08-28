using Discord;
using Serilog;

namespace indy_scca_discord_bot.Util
{
    public static class LogHandler
    {
        public static Task LogMessages(LogMessage messages)
        {
            string formattedMessage = (messages.Source != null && messages.Message != null) ?
                $"Discord:\t{messages.Source}\t{messages.Message}" :
                $"Discord:\t{messages.ToString()}";
            switch (messages.Severity)
            {
                case LogSeverity.Critical:
                    Log.Fatal(formattedMessage);
                    break;
                case LogSeverity.Error:
                    Log.Error(formattedMessage);
                    break;
                case LogSeverity.Warning:
                    Log.Warning(formattedMessage);
                    break;
                case LogSeverity.Info:
                    Log.Information(formattedMessage);
                    break;
                case LogSeverity.Verbose:
                    Log.Verbose(formattedMessage);
                    break;
                default:
                    Log.Information($"Log Severity: {messages.Severity}");
                    Log.Information(formattedMessage);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
