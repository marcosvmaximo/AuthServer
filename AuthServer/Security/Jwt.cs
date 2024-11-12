using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthServer.Users;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer.Security;

    public class JwtService : IJwtService
    {
        private const string SECRET = "0e5582adfb7fa6bb770815f3c6b3534d311bd5fe";
        private const long EXPIRE_HOURS = 48L;
        private const long ADMIN_EXPIRE_HOURS = 2L;
        private const string ISSUER = "PUCPR AuthServer";
        private const string USER_FIELD = "User";

        private readonly ILogger<JwtService> _logger;

        public JwtService(ILogger<JwtService> logger)
        {
            _logger = logger;
        }

        public string CreateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(user.IsAdmin ? ADMIN_EXPIRE_HOURS : EXPIRE_HOURS);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, ISSUER),
                new Claim(USER_FIELD, SerializeUser(user))
            };

            // Adicionando as roles como claims
            if (user.Roles != null)
            {
                claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));
            }

            var tokenDescriptor = new JwtSecurityToken(
                issuer: ISSUER,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public ClaimsPrincipal Extract(HttpRequest request)
        {
            try
            {
                if (!request.Headers.ContainsKey("Authorization") || 
                    !request.Headers["Authorization"].ToString().StartsWith("Bearer "))
                {
                    return null;
                }

                var token = request.Headers["Authorization"].ToString().Substring("Bearer ".Length);
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(SECRET);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = ISSUER,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ValidateAudience = false
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                if (validatedToken is JwtSecurityToken jwtToken && jwtToken.Issuer != ISSUER)
                {
                    _logger.LogDebug("Token rejected: {Expected} != {Actual}", ISSUER, jwtToken.Issuer);
                    return null;
                }

                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogDebug("Token rejected", ex);
                return null;
            }
        }

        private string SerializeUser(User user)
        {
            return System.Text.Json.JsonSerializer.Serialize(new { user.Id, user.Roles });
        }
    }

    public interface IJwtService
    {
        string CreateToken(User user);
        ClaimsPrincipal Extract(HttpRequest request);
    }