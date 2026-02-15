namespace envmanager.src.services.interfaces.project
{
    public interface IUpdateProjectDescriptionUseCase
    {
        public Task<bool> Execute(string decription, string projecetId);
    }
}
