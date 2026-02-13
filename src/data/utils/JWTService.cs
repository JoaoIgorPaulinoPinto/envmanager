using envmanager.src.data.service.schemes;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace envmanager.src.data.utils
{
    public class JWTService
    {
        private readonly string _key;
        private readonly string _issuer;

        public JWTService(IConfiguration configuration)
        {
            _key = configuration["Jwt:Key"] ?? throw new ArgumentNullException(nameof(configuration), "JWT Key is not configured in appsettings.json.");
            _issuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException(nameof(configuration), "JWT Issueris not configured in appsettings.json.");
        }

        public string CreateUserToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        public bool IsValid(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_key);


             tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _issuer,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Immediate expiration check
                }, out SecurityToken validatedToken);

            return validatedToken!= null;
        }
    }
}