using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Thunderbird.Application.Services;
using Thunderbird.Domain.Entities;
using Thunderbird.Domain.Models;

namespace Thunderbird.Application.Tests {
    public class TokenServiceTests {
        private static TokenAuthenticationSettings CreateSettings() => new() {
            SecretKey = "this-is-a-test-secret-key-that-is-long-enough",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            CookieName = "access_token",
            ExpiryMinutes = 30
        };

        [Fact]
        public async Task GenerateToken_ProducesTokenValidWithConfiguredKey() {
            var settings = CreateSettings();
            var service = new TokenService(Options.Create(settings));
            var user = new User {
                UserId = 42,
                LoginName = "jdoe",
                LoginPassword = string.Empty,
                FirstName = "John",
                LastName = "Doe",
                Email = "jdoe@example.com",
                PhoneNumber = "+12025550123"
            };

            string token = service.GenerateToken(user);

            var handler = new JsonWebTokenHandler();
            var validationResult = await handler.ValidateTokenAsync(token, new TokenValidationParameters {
                ValidateIssuer = true,
                ValidIssuer = settings.Issuer,
                ValidateAudience = true,
                ValidAudience = settings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey)),
                ValidateLifetime = true
            });

            Assert.True(validationResult.IsValid);
            Assert.Equal("42", validationResult.ClaimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            Assert.Equal("jdoe", validationResult.ClaimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value);
        }

        [Fact]
        public async Task GenerateToken_RejectsWrongSigningKey() {
            var settings = CreateSettings();
            var service = new TokenService(Options.Create(settings));
            var user = new User { UserId = 1, LoginName = "a", LoginPassword = "", FirstName = "A", LastName = "B", Email = "a@example.com", PhoneNumber = "+12025550100" };

            string token = service.GenerateToken(user);

            var handler = new JsonWebTokenHandler();
            var validationResult = await handler.ValidateTokenAsync(token, new TokenValidationParameters {
                ValidateIssuer = true,
                ValidIssuer = settings.Issuer,
                ValidateAudience = true,
                ValidAudience = settings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("a-completely-different-secret-key-value")),
                ValidateLifetime = true
            });

            Assert.False(validationResult.IsValid);
        }
    }
}
