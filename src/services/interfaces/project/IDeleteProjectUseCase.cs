namespace envmanager.src.services.interfaces
{
    public interface IDeleteProjectUseCase
    {
        public Task<bool> Execute( string projectId);
    }
}
