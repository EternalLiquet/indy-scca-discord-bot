using Discord;
using Discord.WebSocket;
using indy_scca_discord_bot.Data;
using Serilog;

namespace indy_scca_discord_bot.EventHandlers
{
    public class MessageDeletedHandler
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly RoleReactionRepository _roleReactionRepository;

        public MessageDeletedHandler(DiscordSocketClient discordSocketClient, RoleReactionRepository roleReactionRepository)
        {
            Log.Information("Instantiating Message Deleted Handler");
            _discordClient = discordSocketClient;
            _roleReactionRepository = roleReactionRepository;
        }

        public async Task InstantiateMessageDeletedHandler()
        {
            Log.Information("Instantiating Message Deleted Handlers");
            _discordClient.MessageDeleted += HandleMessageDeleted;
        }

        private async Task HandleMessageDeleted(Cacheable<IMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel)
        {
            var downloadedMessage = await cachedMessage.GetOrDownloadAsync();
            if (downloadedMessage.Author.Id != _discordClient.CurrentUser.Id)
            {
                return; //If it isn't bot's message then we don't care about it.
            }
            await _roleReactionRepository.DeleteRoleReactionsAsync(downloadedMessage.Id);
        }
    }
}
