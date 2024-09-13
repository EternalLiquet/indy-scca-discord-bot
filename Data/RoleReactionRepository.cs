using indy_scca_discord_bot.Models;
using MongoDB.Driver;

namespace indy_scca_discord_bot.Data
{
    public class RoleReactionRepository
    {
        private readonly IMongoCollection<RoleReaction> _roleReactions;

        public RoleReactionRepository(MongoDbClient mongoDbClient)
        {
            _roleReactions = mongoDbClient.RoleReactions;
        }

        public async Task<RoleReaction> GetRoleReactionAsync(ulong messageId)
        {
            return await _roleReactions.Find<RoleReaction>(roleReaction => roleReaction.MessageId == messageId).FirstOrDefaultAsync();
        }

        public async Task<List<RoleReaction>> GetRoleReactionsAsync(ulong messageId)
        {
            return await _roleReactions.Find<RoleReaction>(roleReaction => roleReaction.MessageId == messageId).ToListAsync();
        }

        public async Task CreateRoleReactionAsync(RoleReaction roleReaction)
        {
            await _roleReactions.InsertOneAsync(roleReaction);
        }

        public async Task DeleteRoleReactionsAsync(ulong messageId)
        {
            await _roleReactions.DeleteManyAsync(roleReaction => roleReaction.MessageId == messageId);
        }
    }
}
