using static envmanager.src.data.service.dtos.UsersDtos;

namespace envmanager.src.services.interfaces.user
{
    public interface IGetUsersUseCase
    {
        Task<List<GetUsersResponse>> Execute();
        Task<GetUsersResponse> Execute(string id);
    }
}
