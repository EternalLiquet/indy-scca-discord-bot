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

        public async Task HandleReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
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

                var guild = (reaction.Channel as SocketGuildChannel).Guild;
                var user = guild.GetUser(reaction.UserId);
                var emoteLookup = guild.Emotes.FirstOrDefault(emote => emote.Name == reaction.Emote.Name);
                if (emoteLookup == null)
                {
                    Log.Debug("Emote not found in guild, must be a unicode emote");
                    var roleId = roleSettings.Find(roleSettings => roleSettings.EmoteId == reaction.Emote.Name).RoleId;
                    var role = guild.Roles.Where(role => role.Id == roleId).FirstOrDefault();
                    await user.AddRoleAsync(role);
                }
                else
                {
                    Log.Debug("Emote found in guild, must be a custom emote");
                    var roleId = roleSettings.Find(roleSettings => roleSettings.EmoteId == emoteLookup.Id.ToString()).RoleId;
                    var role = guild.Roles.Where(role => role.Id == roleId).FirstOrDefault();
                    Log.Debug($"Adding role {role.Name} to user {user.Username}");
                    await user.AddRoleAsync(role);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                Log.Error(e.Message);
            }
        }
        public async Task HandleReactionRemoved(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
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

                var guild = (reaction.Channel as SocketGuildChannel).Guild;
                var user = guild.GetUser(reaction.UserId);
                var emoteLookup = guild.Emotes.FirstOrDefault(emote => emote.Name == reaction.Emote.Name);
                if (emoteLookup == null)
                {
                    Log.Debug("Emote not found in guild, must be a unicode emote");
                    var roleId = roleSettings.Find(roleSettings => roleSettings.EmoteId == reaction.Emote.Name).RoleId;
                    var role = guild.Roles.Where(role => role.Id == roleId).FirstOrDefault();
                    await user.RemoveRoleAsync(role);
                }
                else
                {
                    Log.Debug("Emote found in guild, must be a custom emote");
                    var roleId = roleSettings.Find(roleSettings => roleSettings.EmoteId == emoteLookup.Id.ToString()).RoleId;
                    var role = guild.Roles.Where(role => role.Id == roleId).FirstOrDefault();
                    await user.RemoveRoleAsync(role);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                Log.Error(e.Message);
            }
        }
    }
}
