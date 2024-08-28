using Discord;
using Discord.WebSocket;
using indy_scca_discord_bot.Config;
using indy_scca_discord_bot.EventHandlers;
using indy_scca_discord_bot.Util;
using Serilog;

namespace indy_scca_discord_bot
{
    public class Program
    {
        private static DiscordSocketClient _discordClient;
        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync()
        {
            DotNetEnv.Env.Load();
            DotNetEnv.Env.TraversePath().Load();
            LoggingConfig.CreateLoggerConfiguration();
            _discordClient = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 100,
                AlwaysDownloadUsers = true
            });
            _discordClient.Log += LogHandler.LogMessages;

            await _discordClient.LoginAsync(TokenType.Bot, DotNetEnv.Env.GetString("DISCORD_TOKEN"));
            await _discordClient.StartAsync();

            _discordClient.Ready += () =>
            {
                Log.Information("Bot is now connected!");
                return Task.CompletedTask;
            };

            _discordClient.Disconnected += (Exception e) =>
            {
                Log.Error($"Bot disconnected: {e.Message}");
                _discordClient.LoginAsync(TokenType.Bot, DotNetEnv.Env.GetString("DISCORD_TOKEN"));
                _discordClient.StartAsync();
                return Task.CompletedTask;
            };

            CommandHandler commandHandler = new CommandHandler(_discordClient);
            await commandHandler.InstallCommandsAsync();
            SlashCommandHandler slashCommandHandler = new SlashCommandHandler(_discordClient);

            //_discordClient.SlashCommandExecuted += SlashCommandHandler;

            await Task.Delay(-1);
        }
    }
}
