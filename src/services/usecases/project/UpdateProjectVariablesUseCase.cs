using envmanager.src.data.service.interfaces;
using envmanager.src.data.service.schemes;
using envmanager.src.services.interfaces.project;
using static envmanager.src.data.service.dtos.ProjectDtos;

namespace envmanager.src.services.usecases.project
{
    public class UpdateProjectVariablesUseCase : IUpdateProjectVariablesUseCase
    {
        private readonly IProjectRepository _projectRepository;

        public UpdateProjectVariablesUseCase(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<bool> Execute(UpdateVariablesRequest updateVariablesRequest, string userId)
        {
            if (updateVariablesRequest == null)
                throw new ArgumentException("Request is required.");

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(updateVariablesRequest.project_id))
                throw new ArgumentException("User and project are required.");

            var project = await _projectRepository.GetProjectById(updateVariablesRequest.project_id);
            if (project == null)
                throw new KeyNotFoundException("Project not found.");

            var member = project.Members.FirstOrDefault(m => m.Id == userId);
            var isOwner = project.UserId == userId;
            var isAdminMember = member?.isAdmin == true;
            if (!isOwner && !isAdminMember)
                throw new UnauthorizedAccessException("Only admin members can change this");

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

            return await _projectRepository.UpdateVariables(project.Id, project.Variables);
        }
    }
}
