using envmanager.src.data.infra.db;
using envmanager.src.data.service.dtos;
using envmanager.src.data.service.interfaces;
using envmanager.src.data.service.schemes;
using envmanager.src.data.utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
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
            var projects = await _appDbContext.Projects
                .Find(p => p.UserId == userId)
                .ToListAsync();

            if (projects == null || !projects.Any())
            {
                throw new KeyNotFoundException("No projects found in the database.");
            }

            return projects.Select(p => new GetProjectsResponse
            {
                id = p.Id,
                name = p.ProjectName,
                description = p.Description
            }).ToList();
        }
    }
}
