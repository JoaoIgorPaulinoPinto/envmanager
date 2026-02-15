namespace envmanager.src.services.interfaces.project
{
    public interface IUpdateProjectDescription
    {
        public Task<bool> Execute(string decription, string projecetId);
    }
}
