using static envmanager.src.infra.dtos.UsersDtos;

namespace envmanager.src.services.interfaces.user
{
    public interface ICreateUserUseCase
    {
        Task<string> Execute(CreateUserRequest user);
    }
}
