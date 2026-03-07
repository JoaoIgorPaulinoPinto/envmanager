namespace envmanager.src.services.interfaces.auth
{
    public interface IAuthLogoutUseCase
    {
        Task Execute(string refreshToken);
    }
}
