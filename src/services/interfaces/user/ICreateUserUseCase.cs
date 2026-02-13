using static envmanager.src.data.service.dtos.UsersDtos;

namespace envmanager.src.services.interfaces.user
{
    public interface ICreateUserUseCase
    {
        Task<string> Execute(CreateUserRequest user);
    }
}
