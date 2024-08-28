using Discord;
using Discord.WebSocket;

namespace indy_scca_discord_bot.EventHandlers
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _discordClient;

        public CommandHandler(DiscordSocketClient discordClient)
        {
            _discordClient = discordClient;
        }

        public async Task InstallCommandsAsync()
        {
            _discordClient.SlashCommandExecuted += HandleSlashCommand;
        }

        private async Task HandleSlashCommand(SocketSlashCommand command)
        {
            if (command.CommandName == "setroles")
            {
                var emotes = command.Data.Options.First().Value as string;
                var roles = command.Data.Options.ElementAt(1).Value as string;

                var roleEmotePairs = ParseRolesAndEmotes(emotes, roles);
                var embedBuilder = new EmbedBuilder().WithTitle("React to Get Roles");

                foreach (var pair in roleEmotePairs)
                {
                    embedBuilder.AddField(pair.Value.Name, pair.Key, true);
                }

                var message = await command.Channel.SendMessageAsync(embed: embedBuilder.Build());

                foreach (var pair in roleEmotePairs.Keys)
                {
                    var emote = Emote.Parse(pair);
                    await message.AddReactionAsync(emote);
                }

                _discordClient.ReactionAdded += async (cachedMessage, channel, reaction) =>
                {
                    if (reaction.MessageId == message.Id)
                    {
                        var role = roleEmotePairs[reaction.Emote.ToString()];
                        var guildUser = reaction.User.IsSpecified ? reaction.User.Value as IGuildUser : null;

                        if (guildUser != null)
                        {
                            await guildUser.AddRoleAsync(role);
                        }
                    }
                };
            }
        }

        private Dictionary<string, IRole> ParseRolesAndEmotes(string emotes, string roles)
        {
            var emoteList = emotes.Split(',');
            var roleList = roles.Split(',');
            var pairs = new Dictionary<string, IRole>();

            for (int i = 0; i < emoteList.Length; i++)
            {
                var emote = emoteList[i].Trim();
                var roleName = roleList[i].Trim();
                var role = _discordClient.Guilds.First().Roles.FirstOrDefault(r => r.Name == roleName);

                if (role != null)
                {
                    pairs.Add(emote, role);
                }
            }

            return pairs;
        }
    }
}
