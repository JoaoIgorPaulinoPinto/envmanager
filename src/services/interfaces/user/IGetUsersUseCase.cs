using static envmanager.src.infra.dtos.UsersDtos;

namespace envmanager.src.services.interfaces.user
{
    public interface IGetUsersUseCase
    {
        Task<List<GetUsersResponse>> Execute();
        Task<GetUsersResponse> Execute(string id);
    }
}
