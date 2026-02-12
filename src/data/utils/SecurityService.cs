using Microsoft.AspNetCore.Identity;

namespace envmanager.src.data.utils
{
    public class SecurityService
    {
        private readonly PasswordHasher<string> _passwordHasher = new PasswordHasher<string>();

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.");
            return _passwordHasher.HashPassword("user-context", password);
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword) || string.IsNullOrWhiteSpace(providedPassword))
                return false;

            var result = _passwordHasher.VerifyHashedPassword("user-context", hashedPassword, providedPassword);

            return result == PasswordVerificationResult.Success;
        }
    }
}