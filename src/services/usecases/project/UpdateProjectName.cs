using envmanager.src.data.service.interfaces;
using envmanager.src.services.interfaces.project;

namespace envmanager.src.services.usecases.project
{
    public class UpdateProjectName : IUpdateProjectName
    {
        private readonly IProjectRepository _projectRepository;
        public UpdateProjectName(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        public Task<bool> Execute(string name, string projecetId)
        {
           return _projectRepository.UpdateName(name, projecetId);
        }
    }
}
