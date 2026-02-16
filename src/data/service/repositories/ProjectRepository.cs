using envmanager.src.data.infra.db;
using envmanager.src.data.service.interfaces;
using envmanager.src.data.service.schemes;
using MongoDB.Bson;
using MongoDB.Driver;

namespace envmanager.src.data.service.repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;

        public ProjectRepository(AppDbContext db)
        {
            _context = db;
        }

        public async Task<bool> CreateProject(Project project)
        {
            await _context.Projects.InsertOneAsync(project);
            return true;
        }

        public async Task<List<Project>> GetProjectsByUser(string userId)
        {
            var filter = Builders<Project>.Filter.Or(
                 Builders<Project>.Filter.Eq(p => p.UserId, userId),
                 Builders<Project>.Filter.ElemMatch(p => p.Members, m => m.Id == userId)
             );

            return await _context.Projects.Find(filter).ToListAsync();
        }

        public async Task<Project?> GetProjectById(string projectId)
        {
            return await _context.Projects
                .Find(p => p.Id == projectId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateVariables(string projectId, List<Key> variables)
        {
            var filter = Builders<Project>.Filter.Eq(p => p.Id, projectId);
            var update = Builders<Project>.Update.Set(p => p.Variables, variables);
            var result = await _context.Projects.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateName(string name, string projectId)
        {
            var filter = Builders<Project>.Filter.Eq(p => p.Id, projectId);
            var update = Builders<Project>.Update.Set(p => p.ProjectName, name);
            var result = await _context.Projects.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateDescription(string description, string projectId)
        {
            var filter = Builders<Project>.Filter.Eq(p => p.Id, projectId);
            var update = Builders<Project>.Update.Set(p => p.Description, description);
            var result = await _context.Projects.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> SetMemberAdmin(string projectId, string userId, bool isAdmin)
        {
            var filter = Builders<Project>.Filter.Eq(p => p.Id, projectId);
            var update = Builders<Project>.Update.Set("Members.$[m].isAdmin", isAdmin);

            var options = new UpdateOptions
            {
                ArrayFilters = new[]
                {
                    new BsonDocumentArrayFilterDefinition<Project>(new BsonDocument("m.Id", userId))
                }
            };

            var result = await _context.Projects.UpdateOneAsync(filter, update, options);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> AddMember(string projectId, ProjectMember member)
        {
            var filter = Builders<Project>.Filter.Eq(p => p.Id, projectId);
            var update = Builders<Project>.Update.Push(p => p.Members, member);
            var result = await _context.Projects.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
    }
}
