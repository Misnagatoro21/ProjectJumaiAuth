
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectJumaiAuth.Extensions;
using ProjectJumaiAuth.Mejoras;
using ProjectJumaiAuth.Models;
using ProjectJumaiAuth.Models.Settings;
using ProjectJumaiAuth.Servicces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ProjectJumaiAuth.Services
{
    public class TokenService(IOptions<JwtSettings> _JwtSettings) : ITokenServices
    {
        private readonly JwtSettings _jwt = _JwtSettings.Value;
        public string GenerateAccessToken(Usuario usuario)
        {
            var claims = new List<Claim>
            {
              new (ClaimTypes.NameIdentifier,usuario.Id.ToString()),
              new (ClaimTypes.Name, usuario.NombreUsuario),
               new (ClaimTypes.Email, usuario.Email),
                new (ClaimTypes.Role,usuario.Rol?.Nombre??"Usuario")
            };
            foreach (var permission in EffectivePermission.FromUser(usuario))
            {
                claims.Add(new Claim("permission", permission));
            
        }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(

                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwt.TokenExpirationInMinutes),
                signingCredentials: creds

                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GeneratePasswordResetToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
