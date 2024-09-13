using Discord.WebSocket;
using indy_scca_discord_bot.Services;
using Serilog;

namespace indy_scca_discord_bot.EventHandlers
{
    public class ReactHandler
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly RoleReactionService _roleReactionService;

        public ReactHandler(DiscordSocketClient discordSocketClient, RoleReactionService roleReactionService)
        {
            Log.Information("Instantiating React Handler");
            _discordClient = discordSocketClient;
            _roleReactionService = roleReactionService;
        }

        public async Task InstantiateReactHandlers()
        {
            Log.Information("Instantiating React Handlers");
            _discordClient.ReactionAdded += _roleReactionService.HandleReactionAdded;
            _discordClient.ReactionRemoved += _roleReactionService.HandleReactionRemoved;
            Log.Information("React Handlers instantiated");
        }
    }
}
