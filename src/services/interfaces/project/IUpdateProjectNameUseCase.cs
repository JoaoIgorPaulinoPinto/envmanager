namespace envmanager.src.services.interfaces.project
{
    public interface IUpdateProjectNameUseCase
    {
        public Task<bool> Execute(string decription, string projecetId);
    }
}
