using envmanager.src.data.service.schemes;
using MongoDB.Driver;

namespace envmanager.src.data.infra.db
{
 
        public class AppDbContext
        {
            private readonly IMongoDatabase _database;

            public AppDbContext(IConfiguration configuration)
            {
                var connectionString = configuration.GetConnectionString("MongoDb");
                var client = new MongoClient(connectionString);

                _database = client.GetDatabase(configuration.GetConnectionString("DataBaseName"));
            }

            public IMongoCollection<User> Users => _database.GetCollection<User>("user");
            public IMongoCollection<Project> Projects => _database.GetCollection<Project>("project");
    }
}
