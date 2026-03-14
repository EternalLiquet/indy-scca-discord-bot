using Discord;
using Discord.WebSocket;
using indy_scca_discord_bot.Data;
using Serilog;

namespace indy_scca_discord_bot.Services
{
    public class RoleReactionService
    {
        private readonly RoleReactionRepository _roleReactionRepository;
        private readonly DiscordSocketClient _discordClient;

        public RoleReactionService(DiscordSocketClient discordClient, RoleReactionRepository roleReactionRepository)
        {
            _roleReactionRepository = roleReactionRepository;
            _discordClient = discordClient;
        }

        public Task HandleReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
            => HandleRoleReactionChanged(cachedMessage, reaction, shouldAddRole: true);

        public Task HandleReactionRemoved(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
            => HandleRoleReactionChanged(cachedMessage, reaction, shouldAddRole: false);

        private async Task HandleRoleReactionChanged(Cacheable<IUserMessage, ulong> cachedMessage, SocketReaction reaction, bool shouldAddRole)
        {
            try
            {
                var downloadedMessage = await cachedMessage.GetOrDownloadAsync();
                if (downloadedMessage.Author.Id != _discordClient.CurrentUser.Id)
                {
                    return; //If it isn't bot's message then we don't care about it.
                }

                if (downloadedMessage.Author.Id == reaction.UserId)
                {
                    return; //If the bot is the one reacting, we ignore this too
                }
                var roleSettings = await _roleReactionRepository.GetRoleReactionsAsync(reaction.MessageId);
                if (roleSettings == null || roleSettings.Count == 0)
                {
                    return;
                }

                if (reaction.Channel is not SocketGuildChannel guildChannel)
                {
                    Log.Warning("Ignoring reaction for message {MessageId} because channel {ChannelId} is not a guild channel", reaction.MessageId, reaction.Channel.Id);
                    return;
                }

                var guild = guildChannel.Guild;
                var emoteId = GetReactionEmoteId(reaction);
                var roleSetting = roleSettings.FirstOrDefault(setting => setting.EmoteId == emoteId);
                if (roleSetting == null)
                {
                    Log.Warning("No role reaction mapping found for message {MessageId} and emote {EmoteId}", reaction.MessageId, emoteId);
                    return;
                }

                var user = await GetReactingUserAsync(guild, reaction.UserId);
                if (user == null)
                {
                    Log.Warning("Unable to resolve reacting user {UserId} in guild {GuildId}", reaction.UserId, guild.Id);
                    return;
                }

                if (shouldAddRole)
                {
                    await user.AddRoleAsync(roleSetting.RoleId);
                    Log.Debug("Added role {RoleId} to user {UserId} for message {MessageId}", roleSetting.RoleId, reaction.UserId, reaction.MessageId);
                }
                else
                {
                    await user.RemoveRoleAsync(roleSetting.RoleId);
                    Log.Debug("Removed role {RoleId} from user {UserId} for message {MessageId}", roleSetting.RoleId, reaction.UserId, reaction.MessageId);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to process reaction change for message {MessageId} and user {UserId}", reaction.MessageId, reaction.UserId);
            }
        }

        private static string GetReactionEmoteId(SocketReaction reaction)
            => reaction.Emote is Emote customEmote ? customEmote.Id.ToString() : reaction.Emote.Name;

        private async Task<IGuildUser?> GetReactingUserAsync(SocketGuild guild, ulong userId)
        {
            var cachedUser = guild.GetUser(userId);
            if (cachedUser != null)
            {
                return cachedUser;
            }

            var restGuild = await _discordClient.Rest.GetGuildAsync(guild.Id);
            return await restGuild.GetUserAsync(userId);
        }
    }
}
