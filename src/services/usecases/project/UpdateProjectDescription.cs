using envmanager.src.data.service.interfaces;
using envmanager.src.services.interfaces.project;
using System.Xml.Linq;

namespace envmanager.src.services.usecases.project
{
    public class UpdateProjectDescription : IUpdateProjectDescription
    {
        private readonly IProjectRepository _projectRepository;
        public UpdateProjectDescription(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        public Task<bool> Execute(string decription, string projecetId)
        {
            return _projectRepository.UpdateDescription(decription, projecetId);

        }
    }
}
