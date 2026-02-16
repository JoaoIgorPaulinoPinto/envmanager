namespace envmanager.src.services.interfaces.project
{
    public interface IUpdateProjectDescriptionUseCase
    {
        Task<bool> Execute(string description, string projectId, string userId);
    }
}
