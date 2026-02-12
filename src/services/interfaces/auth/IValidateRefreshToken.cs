namespace envmanager.src.services.interfaces.auth
{
    public interface IValidateRefreshToken
    {
        public Task<string> Execute(string refreshToken);
    }
}
