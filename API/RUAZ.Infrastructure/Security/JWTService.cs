using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RUAZ.Application.Contracts;
using RUAZ.Application.Interfaces;
using RUAZ.Domain.Entities;
using RUAZ.Domain.Enums;

namespace RUAZ.Infrastructure.Security
{
    public class JWTService(
        IConfiguration configuration) : IJWTService
    {
        public LoginResponse GenerateAcessToken(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var key = GetRequiredSetting("Jwt:Key");
            var issuer = GetRequiredSetting("Jwt:Issuer");
            var audience = GetRequiredSetting("Jwt:Audience");
            var keyBytes = Encoding.UTF8.GetBytes(key);

            if (keyBytes.Length < 32)
            {
                throw new InvalidOperationException("Jwt:Key deve conter pelo menos 32 bytes para HS256.");
            }

            if (!int.TryParse(configuration["Jwt:ExpiresInMinutes"], out var expiresInMinutes)
                || expiresInMinutes <= 0)
            {
                throw new InvalidOperationException("Jwt:ExpiresInMinutes deve ser um inteiro positivo.");
            }

            var now = DateTime.UtcNow;
            var expiresAt = now.AddMinutes(expiresInMinutes);

            // User ainda não possui uma role persistida; emitir somente a permissão comum.
            var role = Roles.User.ToString();
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, role)
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: now,
                expires: expiresAt,
                signingCredentials: credentials);

            return new LoginResponse
            {
                TokenJWT = new JwtSecurityTokenHandler().WriteToken(token),
                Role = role,
                ExpiresIn = ((long)(expiresAt - now).TotalSeconds)
                    .ToString(CultureInfo.InvariantCulture)
            };
        }

        private string GetRequiredSetting(string name)
        {
            var value = configuration[name];

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"Configuração obrigatória ausente: {name}.");
            }

            return value;
        }
    }
}
