using indy_scca_discord_bot.Models;
using MongoDB.Driver;
using Serilog;

namespace indy_scca_discord_bot.Data
{
    public class MongoDbClient
    {

        public readonly IMongoDatabase _mongoDatabase;

        public MongoDbClient(string connectionString, string databaseName)
        {
            Log.Information("Instantiating Database Connection");
            var client = new MongoClient(connectionString);
            _mongoDatabase = client.GetDatabase(databaseName);
            Log.Information("Database Connection complete");
        }

        public IMongoCollection<RoleReaction> RoleReactions => _mongoDatabase.GetCollection<RoleReaction>("RoleReactions");
    }
}
