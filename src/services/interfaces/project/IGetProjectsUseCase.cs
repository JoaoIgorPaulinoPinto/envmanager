using static envmanager.src.data.service.dtos.ProjectDtos;

namespace envmanager.src.services.interfaces.project
{
    public interface IGetProjectsUseCase
    {
        public Task<List<GetProjectsResponse>> Execute(string userId);
    }
}
