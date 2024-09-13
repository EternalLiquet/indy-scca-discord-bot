using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace indy_scca_discord_bot.Models
{
    public class RoleReaction
    {
        [BsonId]
        public ObjectId id;
        [BsonElement("messageId")]
        public ulong MessageId { get; set; }

        [BsonElement("roleId")]
        public ulong RoleId { get; set; }

        [BsonElement("emoteId")]
        public String EmoteId { get; set; }

        [BsonElement("guildId")]
        public ulong GuildId { get; set; }

        [BsonElement("channelId")]
        public ulong ChannelId { get; set; }
    }
}
