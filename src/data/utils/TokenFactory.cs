using envmanager.src.data.service.schemes;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace envmanager.src.data.utils
{
    public interface ITokenFactory
    {
        string CreateUserToken(User user);
        string CreateInviteToken(string inviterId, string invitedId, string projectId);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromToken(string token);
    }

    public class TokenFactory : ITokenFactory
    {
        private readonly string _key;
        private readonly string _issuer;

        public TokenFactory(IConfiguration configuration)
        {
            _key = configuration["Jwt:Key"] ?? throw new ArgumentNullException("JWT Key missing.");
            _issuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("JWT Issuer missing.");
        }

        // --- Core Private Helper ---
        private string GenerateToken(IEnumerable<Claim> claims, DateTime expires)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _issuer,
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
            
        // --- Specialized Factory Methods ---

        public string CreateUserToken(User user)
        {
            var claims = new[] {
                new Claim("id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            return GenerateToken(claims, DateTime.UtcNow.AddMinutes(15));
        }

        public string CreateInviteToken(string inviterId, string invitedId, string projectId)
        {
            var claims = new[] {
                new Claim("inviterId", inviterId),
                new Claim("invitedId", invitedId),
                new Claim("projectId", projectId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            return GenerateToken(claims, DateTime.UtcNow.AddHours(24));
        }

        public string GenerateRefreshToken() => Guid.NewGuid().ToString("N");

        // Returns the payload if valid, or null if tampered/expired
        public ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                return handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _issuer,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);
            }
            catch { return null; }
        }
    }
}