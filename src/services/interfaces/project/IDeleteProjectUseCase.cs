namespace envmanager.src.services.interfaces.project
{
    public interface IDeleteProjectUseCase
    {
        public Task<bool> Execute( string projectId);
    }
}
