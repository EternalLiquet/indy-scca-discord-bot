using Discord;
using Discord.WebSocket;
using indy_scca_discord_bot.Data;
using indy_scca_discord_bot.Models;
using MongoDB.Driver.Linq;

namespace indy_scca_discord_bot.EventHandlers
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly RoleReactionRepository _roleReactionRepository;

        public CommandHandler(DiscordSocketClient discordClient, RoleReactionRepository roleReactionRepository)
        {
            _discordClient = discordClient;
            _roleReactionRepository = roleReactionRepository;
        }

        public async Task InstallCommandsAsync()
        {
            _ = Task.Factory.StartNew(() => { _discordClient.SlashCommandExecuted += HandleSlashCommand; });
        }

        private async Task HandleSlashCommand(SocketSlashCommand command)
        {
            if (command.CommandName == "configureroles")
            {
                var roleEmotePairs = command.Data.Options.First().Value as string;
                var guild = _discordClient.GetGuild(command.GuildId.Value);
                Console.WriteLine(roleEmotePairs);
                var pairs = await ParseRolesAndEmotesAsync(roleEmotePairs, guild);


                EmbedBuilder roleEmbedMessage = new EmbedBuilder();
                roleEmbedMessage.WithTitle("Role Assignment");
                foreach (var pair in pairs)
                {
                    roleEmbedMessage.AddField(field =>
                    {
                        try { var emote = guild.Emotes.FirstOrDefault(e => e.Id.ToString() == pair.Key); field.Name = $"{emote}"; }
                        catch (Exception) { var emote = pair.Key; field.Name = $"{emote}"; }
                        field.Value = $"<@&{pair.Value}>";
                        field.IsInline = true;
                    });
                }
                if (command.Data.Options.Count() > 1)
                {
                    roleEmbedMessage.WithFooter(footer => footer.Text = $"{command.Data.Options.ElementAt(1).Value as string}");
                }
                var finishedEmbed = roleEmbedMessage.Build();
                var messageToListen = await command.Channel.SendMessageAsync(embed: finishedEmbed);
                foreach (var pair in pairs)
                {
                    var emote = guild.Emotes.FirstOrDefault(e => e.Id.ToString() == pair.Key);
                    if (emote == null)
                    {
                        await messageToListen.AddReactionAsync(new Emoji(pair.Key));
                    }
                    else
                    {
                        await messageToListen.AddReactionAsync(emote);
                    }
                    Thread.Sleep(2000);
                }

                foreach (var pair in pairs)
                {
                    await _roleReactionRepository.CreateRoleReactionAsync(new RoleReaction() { MessageId = messageToListen.Id, ChannelId = command.ChannelId.Value, GuildId = command.GuildId.Value, EmoteId = pair.Key, RoleId = pair.Value });
                }
                // Moving this down here as using RespondAsync will terminate the rest of the method
                await command.RespondAsync("Roles have been set up", ephemeral: true);
            }
        }

        private async Task<Dictionary<string, ulong>> ParseRolesAndEmotesAsync(string roleEmotePairs, SocketGuild guild)
        {
            Dictionary<string, ulong> pairs = new Dictionary<string, ulong>();
            String[] roleEmotePairList = roleEmotePairs.Split(',');
            foreach (String pair in roleEmotePairList)
            {
                String[] roleEmoteSplit = pair.Split('+');
                String role = roleEmoteSplit[0].Trim();
                String emote = roleEmoteSplit[1].Trim();
                Console.WriteLine(role);
                var roleLookedUp = guild.Roles.Where(roleToSearch => role.Contains(roleToSearch.Id.ToString()));
                Console.WriteLine("Role found: " + roleLookedUp.First().Name);
                Console.WriteLine(emote);
                try
                {
                    var emoteLookedUp = guild.Emotes.Where(emoteToSearch => emote.Contains(emoteToSearch.Id.ToString()));
                    Console.WriteLine("Emote found: " + emoteLookedUp.First().Name);
                    pairs.Add(emoteLookedUp.First().Id.ToString(), roleLookedUp.First().Id);

                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("Emote not found");
                    pairs.Add(emote, roleLookedUp.First().Id);
                }
            }
            return pairs;
        }
    }
}
