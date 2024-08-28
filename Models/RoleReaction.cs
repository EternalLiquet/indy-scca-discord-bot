using MongoDB.Bson.Serialization.Attributes;

namespace indy_scca_discord_bot.Models
{
    public class RoleReaction
    {
        [BsonId]
        [BsonElement("messageId")]
        public ulong MessageId { get; set; }

        [BsonElement("roleId")]
        public ulong RoleId { get; set; }

        [BsonElement("emoteId")]
        public ulong EmoteId { get; set; }

        [BsonElement("guildId")]
        public ulong GuildId { get; set; }

        [BsonElement("channelId")]
        public ulong ChannelId { get; set; }
    }
}
