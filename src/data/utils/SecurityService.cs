namespace envmanager.src.data.utils
{
    using Microsoft.AspNetCore.Identity;

    public class SecurityService
    {
        private readonly PasswordHasher<string> _passwordHasher = new PasswordHasher<string>();

        public string GerarHash(string password)
        {
            return _passwordHasher.HashPassword("user-identity-context", password);
        }

        public bool VerificarSenha(string hashedPw, string providedPw)
        {
            var result = _passwordHasher.VerifyHashedPassword("user-identity-context", hashedPw, providedPw);

            return result == PasswordVerificationResult.Success;
        }
    }
}
