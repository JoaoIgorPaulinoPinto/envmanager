namespace envmanager.src.services.interfaces.project
{
    public interface IUpdateProjectNameUseCase
    {
        Task<bool> Execute(string name, string projectId, string userId);
    }
}
