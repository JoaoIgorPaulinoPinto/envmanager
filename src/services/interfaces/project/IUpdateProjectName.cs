namespace envmanager.src.services.interfaces.project
{
    public interface IUpdateProjectName
    {
        public Task<bool> Execute(string decription, string projecetId);
    }
}
