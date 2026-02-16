using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace envmanager.src.data.utils
{
    public class SecurityService
    {
        private readonly PasswordHasher<string> _passwordHasher = new PasswordHasher<string>();

        private const string UserContext = "user-context";

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.");

            string preHashedPassword = GeneratePreHash(password);
            return _passwordHasher.HashPassword(UserContext, preHashedPassword);
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword) || string.IsNullOrWhiteSpace(providedPassword))
                return false;

            string preHashedProvided = GeneratePreHash(providedPassword);

            var result = _passwordHasher.VerifyHashedPassword(UserContext, hashedPassword, preHashedProvided);

            return result == PasswordVerificationResult.Success;
        }

        private string GeneratePreHash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}