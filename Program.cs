using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using indy_scca_discord_bot.Config;
using indy_scca_discord_bot.Data;
using indy_scca_discord_bot.EventHandlers;
using indy_scca_discord_bot.Services;
using indy_scca_discord_bot.Util;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace indy_scca_discord_bot
{
    public class Program
    {
        private static DiscordSocketClient _discordClient;
        private CommandService _commandService;
        private IServiceProvider _serviceProvider;
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
                return Task.CompletedTask;
            };

            _commandService = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                CaseSensitiveCommands = false,
            });

            _serviceProvider = new Microsoft.Extensions.DependencyInjection.ServiceCollection()
                .AddSingleton(_discordClient)
                .AddSingleton(_commandService)
                .AddSingleton<InteractionService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton(new MongoDbClient(DotNetEnv.Env.GetString("MONGODB_URI"), DotNetEnv.Env.GetString("MONGODB_DB_NAME")))
                .AddSingleton<RoleReactionRepository>()
                .AddSingleton<RoleReactionService>()
                .AddSingleton<ReactHandler>()
                .AddSingleton<MessageDeletedHandler>()
            .BuildServiceProvider();

            var commandHandler = _serviceProvider.GetRequiredService<CommandHandler>();
            var reactHandler = _serviceProvider.GetRequiredService<ReactHandler>();
            var messageDeletedHandler = _serviceProvider.GetRequiredService<MessageDeletedHandler>();

            await commandHandler.InstallCommandsAsync();
            await reactHandler.InstantiateReactHandlers();
            await messageDeletedHandler.InstantiateMessageDeletedHandler();
            SlashCommandInitializer slashCommandHandler = new SlashCommandInitializer(_discordClient);

            //_discordClient.SlashCommandExecuted += SlashCommandHandler;

            await Task.Delay(-1);
        }
    }
}
