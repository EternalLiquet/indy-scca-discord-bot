using Discord;
using Discord.WebSocket;

namespace indy_scca_discord_bot.EventHandlers
{
    public class SlashCommandHandler
    {
        private readonly DiscordSocketClient _client;

        public SlashCommandHandler(DiscordSocketClient client)
        {
            _client = client;
            _client.Ready += RegisterCommands;
        }

        private async Task RegisterCommands()
        {
            var guild = _client.GetGuild(1141089564623122532); // Replace with your Guild ID

            var setRolesCommand = new SlashCommandBuilder()
                .WithName("setroles")
                .WithDescription("Set roles with emotes")
                .AddOption("emotes", ApplicationCommandOptionType.String, "Comma-separated list of emotes", true)
                .AddOption("roles", ApplicationCommandOptionType.String, "Comma-separated list of roles", true);

            try
            {
                await guild.CreateApplicationCommandAsync(setRolesCommand.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering command: {ex.Message}");
            }
        }
    }
}
