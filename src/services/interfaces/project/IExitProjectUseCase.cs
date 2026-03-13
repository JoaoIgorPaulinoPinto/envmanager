namespace envmanager.src.services.interfaces.project
{
    public interface IExitProjectUseCase
    {
        public Task<bool> Execute(string projectId, string userId);
    }
}
