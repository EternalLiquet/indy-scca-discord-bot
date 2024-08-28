using indy_scca_discord_bot.Data;
using indy_scca_discord_bot.Models;

namespace indy_scca_discord_bot.Services
{
    public class RoleReactionService
    {
        private readonly RoleReactionRepository _roleReactionRepository;

        public RoleReactionService(RoleReactionRepository roleReactionRepository)
        {
            _roleReactionRepository = roleReactionRepository;
        }

        public async Task<RoleReaction> GetRoleReactionAsync(ulong messageId)
        {
            return await _roleReactionRepository.GetRoleReactionAsync(messageId);
        }

        public async Task CreateRoleReactionAsync(RoleReaction roleReaction)
        {
            await _roleReactionRepository.CreateRoleReactionAsync(roleReaction);
        }

        public async Task DeleteRoleReactionAsync(ulong messageId)
        {
            await _roleReactionRepository.DeleteRoleReactionAsync(messageId);
        }
    }
}
