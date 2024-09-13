using Discord;
using Discord.WebSocket;
using Serilog;

namespace indy_scca_discord_bot.Config
{
    public class SlashCommandInitializer
    {
        private readonly DiscordSocketClient _client;

        public SlashCommandInitializer(DiscordSocketClient client)
        {
            _client = client;
            _client.Ready += RegisterCommands;
        }

        private async Task RegisterCommands()
        {
            Log.Information("Registering commands");
            var guild = _client.GetGuild(1141089564623122532); // Replace with your Guild ID

            Log.Information("Getting guild");


            var setRolesCommand = new SlashCommandBuilder()
                .WithName("configureroles")
                .WithDescription("Set roles with emotes")
                .AddOption(new SlashCommandOptionBuilder().WithName("role-emote-pairs").WithType(ApplicationCommandOptionType.String).WithDescription("\"@Role + :emote:\" to set up, separated by commas").WithRequired(true))
                .AddOption(new SlashCommandOptionBuilder().WithName("role-group-description").WithDescription("You can describe what these roles are for, this will show up at the bottom of the message").WithType(ApplicationCommandOptionType.String).WithRequired(false));
            Log.Information("Commands ready to register");

            try
            {
                if (DotNetEnv.Env.GetString("ENVIRONMENT") == "prod")
                {
                    await _client.CreateGlobalApplicationCommandAsync(setRolesCommand.Build());
                }
                else
                {
                    await guild.CreateApplicationCommandAsync(setRolesCommand.Build());
                }
                Console.WriteLine("Commands registered");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering command: {ex.Message}");
            }
        }
    }
}
