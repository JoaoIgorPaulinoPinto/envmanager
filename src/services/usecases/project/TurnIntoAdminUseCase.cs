using envmanager.src.data.service.dtos;
using envmanager.src.data.service.interfaces;
using envmanager.src.services.interfaces.project;

namespace envmanager.src.services.usecases.project
{
    public class TurnIntoAdminUseCase : ITurnIntoAdminUseCase
    {
        private readonly IProjectRepository _projectRepository;
        public TurnIntoAdminUseCase(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }


        public Task<bool> Execute(ProjectDtos.TurnIntoAdminRequest request, string adminId)
        {
          return _projectRepository.TurnIntoAdmin(request, adminId);
        }
    }
}
