using static envmanager.src.data.service.dtos.ProjectDtos;

namespace envmanager.src.services.interfaces.project
{
    public interface ITurnIntoAdminUseCase
    {
        public Task<bool> Execute(ToggleMemberAdminRequest request, string adminId);
    }
}
