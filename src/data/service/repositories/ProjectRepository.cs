using envmanager.src.data.infra.db;
using envmanager.src.data.service.interfaces;
using envmanager.src.data.service.schemes;
using envmanager.src.data.utils;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Xml.Linq;
using static envmanager.src.data.service.dtos.KeyDTos;
using static envmanager.src.data.service.dtos.MemberDtos;
using static envmanager.src.data.service.dtos.ProjectDtos;
using static GlobalExceptionHandler;

namespace envmanager.src.data.service.repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;
        private readonly SecurityService _secService;

        public ProjectRepository(AppDbContext db, SecurityService securityService)
        {
            _secService = securityService;
            _context = db;
        }

        public async Task<bool> CreateProject(CreateProjectRequest createProjectRequest, string userId)
        {
            Project project = new Project()
            {
                Description = createProjectRequest.description,
                ProjectName = createProjectRequest.name,
                Password = createProjectRequest.password != "" ? _secService.HashPassword(createProjectRequest.password!) : "",
                UserId = userId,
                Members = [new ProjectMember { Id = userId , isAdmin = true}],
            };
            await _context.Projects.InsertOneAsync(project);
            return true;
        }

        public async Task<List<GetProjectsResponse>> GetProjects(string userId)
        {
            var filter = Builders<Project>.Filter.Or(
                 Builders<Project>.Filter.Eq(p => p.UserId, userId),
                 Builders<Project>.Filter.ElemMatch(p => p.Members, m => m.Id == userId)
             );

            var projects = await _context.Projects.Find(filter).ToListAsync();
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
                need_password = !string.IsNullOrEmpty(p.Password),
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

            var project = await _context.Projects.Find(filter).FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(project.Password)) throw new BusinessException("Password is required to access this project");

            if (project == null)
            {
                throw new KeyNotFoundException("Project not found.");
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
        public async Task<GetProjectByIdResponse> GetProjectById(string userId, string projId, string password)
        {

            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException("Password is required to access this project");

            var filter = Builders<Project>.Filter.And(
                Builders<Project>.Filter.Eq(p => p.Id, projId),
                Builders<Project>.Filter.Or(
                    Builders<Project>.Filter.Eq(p => p.UserId, userId),
                    Builders<Project>.Filter.ElemMatch(p => p.Members, m => m.Id == userId)
                )
            );

            var project = await _context.Projects.Find(filter).FirstOrDefaultAsync();

            if (project == null )
            {
                throw new KeyNotFoundException("Project not found");
            }
            if(!string.IsNullOrEmpty(project.Password) )
            {
                bool isPasswordValid = _secService.VerifyPassword(project.Password, password);

                if (!isPasswordValid)
                {
                    throw new BusinessException("Access denied. Invalid password.");
                }
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
            var user = await _context.Users.Find(u => u.Id == uId).FirstOrDefaultAsync();
            return user?.UserName ?? "Unknown User";
        }

        public async Task<bool> UpdateVariables(UpdateVariablesRequest updateVariablesRequest, string userId)
        {
            var project = await _context.Projects
                .Find(p => p.UserId == userId && p.Id == updateVariablesRequest.project_id)
                .FirstOrDefaultAsync();


            
            if (project == null) throw new KeyNotFoundException("Project not found.");
            ProjectMember? member = project.Members.Find(u => u.Id == userId);
            if (member == null) throw new Exception("Only admin members can change this");
            bool isAdmin = member.isAdmin;
            if (!isAdmin)
            {
                if (member == null) throw new Exception("Only admin members can change this");
            }
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

            var result = await _context.Projects.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateName(string name, string projId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Project name must be provided and cannot be empty.");
            }

            if (string.IsNullOrEmpty(projId))
            {
                throw new ArgumentException("Project ID must be provided.");
            }

            var filter = Builders<Project>.Filter.Eq(p => p.Id, projId);
            var update = Builders<Project>.Update.Set(p => p.ProjectName, name);

            var result = await _context.Projects.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
            {
                throw new BusinessException("Project not found.");
            }

            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateDescription(string description, string projId)
        {
            {
                if (string.IsNullOrWhiteSpace(description))
                {
                    throw new ArgumentException("Project name must be provided and cannot be empty.");
                }

                if (string.IsNullOrEmpty(projId))
                {
                    throw new ArgumentException("Project ID must be provided.");
                }

                var filter = Builders<Project>.Filter.Eq(p => p.Id, projId);
                var update = Builders<Project>.Update.Set(p => p.Description, description);

                var result = await _context.Projects.UpdateOneAsync(filter, update);

                if (result.MatchedCount == 0)
                {
                    throw new BusinessException("Project not found.");
                }

                return result.ModifiedCount > 0;
            }
        }

        public async Task<bool> TurnIntoAdmin(TurnIntoAdminRequest request, string adminId)
        {
            var project = await _context.Projects.Find(p => p.Id == request.project_id).FirstOrDefaultAsync();

            if (project == null)
            {
                throw new BusinessException("Project not found.");
            }

            var caller = project.Members?.Find(m => m.Id == adminId);
            var target = project.Members?.Find(m => m.Id == request.user_id);

            if (caller == null || (!caller.isAdmin && project.UserId != adminId))
            {
                throw new BusinessException("Only project administrators can promote other members.");
            }

            if (target == null)
            {
                throw new BusinessException("The user to be promoted is not a member of this project.");
            }

            if (target.isAdmin)
            {
                throw new BusinessException("The user is already an administrator.");
            }

            var filter = Builders<Project>.Filter.Eq(p => p.Id, request.project_id);
            var update = Builders<Project>.Update.Set("Members.$[m].isAdmin", true);

            var options = new UpdateOptions
            {
                ArrayFilters = new[]
                {
            new BsonDocumentArrayFilterDefinition<Project>(new BsonDocument("m.Id", request.user_id))
        }
            };

            var result = await _context.Projects.UpdateOneAsync(filter, update, options);

            return result.ModifiedCount > 0;
        }
    }
}
