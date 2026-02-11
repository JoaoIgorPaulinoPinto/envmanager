using envmanager.src.data.schemes;
using MongoDB.Driver;

namespace envmanager.src.infra.db
{
 
        public class AppDbContext
        {
            private readonly IMongoDatabase _database;

            public AppDbContext(IConfiguration configuration)
            {
                var connectionString = configuration.GetConnectionString("MongoDb");
                var client = new MongoClient(connectionString);

                _database = client.GetDatabase("BuildoraDataBase");
            }

            public IMongoCollection<User> Users => _database.GetCollection<User>("user");
    }
}
