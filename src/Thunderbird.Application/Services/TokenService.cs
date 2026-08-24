using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Thunderbird.Application.Interfaces;
using Thunderbird.Domain.Entities;
using Thunderbird.Domain.Models;

namespace Thunderbird.Application.Services {
    public class TokenService : ITokenService {
        private readonly TokenAuthenticationSettings _settings;
        public TokenService(IOptions<TokenAuthenticationSettings> settings) {
            _settings = settings.Value;
        }

        public string GenerateToken(User user) {
            var handler = new JsonWebTokenHandler();
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey)),
                SecurityAlgorithms.HmacSha256);

            var descriptor = new SecurityTokenDescriptor {
                Issuer = _settings.Issuer,
                Audience = _settings.Audience,
                Expires = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
                SigningCredentials = signingCredentials,
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.LoginName),
                    new Claim(ClaimTypes.GivenName, user.FirstName),
                    new Claim(ClaimTypes.Surname, user.LastName)
                })
            };

            return handler.CreateToken(descriptor);
        }
    }
}
