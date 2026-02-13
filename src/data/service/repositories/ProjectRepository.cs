using envmanager.src.data.infra.db;
using envmanager.src.data.service.interfaces;
using envmanager.src.data.service.schemes;
using MongoDB.Driver;
using static envmanager.src.data.service.dtos.KeyDTos;
using static envmanager.src.data.service.dtos.MemberDtos;
using static envmanager.src.data.service.dtos.ProjectDtos;

namespace envmanager.src.data.service.repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _appDbContext;

        public ProjectRepository(AppDbContext db)
        {
            _appDbContext = db;
        }

        public async Task<bool> CreateProject(CreateProjectRequest createProjectRequest, string userId)
        {
            Project project = new Project()
            {
                Description = createProjectRequest.description,
                ProjectName = createProjectRequest.name,
                Password = createProjectRequest.password,
                UserId = userId
            };
            await _appDbContext.Projects.InsertOneAsync(project);
            return true;
        }

        public async Task<List<GetProjectsResponse>> GetProjects(string userId)
        {
            var filter = Builders<Project>.Filter.Or(
                 Builders<Project>.Filter.Eq(p => p.UserId, userId),
                 Builders<Project>.Filter.ElemMatch(p => p.Members, m => m.Id == userId)
             );

            var projects = await _appDbContext.Projects.Find(filter).ToListAsync();
            if (projects == null || !projects.Any())
            {
                throw new KeyNotFoundException("No projects found in the database.");
            }

            return projects.Select(p => new GetProjectsResponse
            {
                id = p.Id,
                name = p.ProjectName,
                description = p.Description,
                isOwner = p.UserId == userId,
            }).ToList();
        }


        /// <summary>
        ///                             Não esquercer de fazer o access_link para entrar no projeto
        /// /summary>
        public async Task<GetProjectByIdResponse> GetProjectById(string userId, string projId)
        {
            var filter = Builders<Project>.Filter.And(
                Builders<Project>.Filter.Eq(p => p.Id, projId),
                Builders<Project>.Filter.Or(
                    Builders<Project>.Filter.Eq(p => p.UserId, userId),
                    Builders<Project>.Filter.ElemMatch(p => p.Members, m => m.Id == userId)
                )
            );

            var project = await _appDbContext.Projects.Find(filter).FirstOrDefaultAsync();

            if (project == null)
            {
                throw new KeyNotFoundException("Project not found or access denied.");
            }

            var memberResponses = new List<GetMemberResponse>();
            foreach (var m in project.Members)
            {
                memberResponses.Add(new GetMemberResponse
                {
                    Name = await GetUserById(m.Id),
                    isAdmin = m.Id == project.UserId 
                });
            }

            return new GetProjectByIdResponse
            {
                id = project.Id,
                name = project.ProjectName,
                description = project.Description,
                variables = project.Variables.Select(v => new GetVariableResponse
                {
                    id = v.Id ?? "",
                    variable = v.Variable,
                    Value = v.Value
                }).ToList(),
                members = memberResponses,
                access_link = project.AccesLink,
                isOwner = userId == project.UserId
            };
        }

        async Task<string> GetUserById(string uId)
        {
            var user = await _appDbContext.Users.Find(u => u.Id == uId).FirstOrDefaultAsync();
            return user?.UserName ?? "Usuário Desconhecido";
        }

        public async Task<bool> UpdateVariables(UpdateVariablesRequest updateVariablesRequest, string userId)
        {
            var project = await _appDbContext.Projects
                .Find(p => p.UserId == userId && p.Id == updateVariablesRequest.project_id)
                .FirstOrDefaultAsync();

            if (project == null) throw new KeyNotFoundException("Project not found.");
            project.Variables ??= new List<Key>();

            foreach (var incomingVar in updateVariablesRequest.variables)
            {
                var existingKey = project.Variables.FirstOrDefault(k => k.Id == incomingVar.id);

                if (existingKey != null)
                {
                    existingKey.Value = incomingVar.Value;
                    existingKey.Variable = incomingVar.variable;
                }
                else
                {
                    project.Variables.Add(new Key
                    {
                        Id = Guid.NewGuid().ToString(),
                        Value = incomingVar.Value,
                        Variable = incomingVar.variable
                    });
                }
            }

            var filter = Builders<Project>.Filter.Eq(p => p.Id, project.Id);
            var update = Builders<Project>.Update.Set(p => p.Variables, project.Variables);

            var result = await _appDbContext.Projects.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }
    }
}
