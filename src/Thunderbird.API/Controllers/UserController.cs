using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

using Thunderbird.API.Models;
using Thunderbird.Application.Interfaces;
using Thunderbird.Domain.Models;

namespace Thunderbird.API.Controllers {
    [EnableRateLimiting("auth")]
    public class UserController : BaseController {
        private readonly IUserService _userService;
        private readonly ICaptchaService _captchaService;
        private readonly ITokenService _tokenService;
        private readonly TokenAuthenticationSettings _tokenSettings;

        public UserController(
            IUserService userService,
            ICaptchaService captchaService,
            ITokenService tokenService,
            IOptions<TokenAuthenticationSettings> tokenSettings) {
            _userService = userService;
            _captchaService = captchaService;
            _tokenService = tokenService;
            _tokenSettings = tokenSettings.Value;
        }

        [HttpPost]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request) {
            bool captchaIsValid = await _captchaService.IsValid(request.CaptchaId, request.CaptchaCode);
            if (!captchaIsValid) {
                return BadRequest("Invalid or expired captcha.");
            }

            var user = await _userService.Login(request.LoginName, request.LoginPassword);
            if (user is null) {
                return Unauthorized();
            }

            string token = _tokenService.GenerateToken(user);
            Response.Cookies.Append(_tokenSettings.CookieName, token, new CookieOptions {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(_tokenSettings.ExpiryMinutes)
            });

            return Ok(new AuthResponse(token, UserResponse.FromUser(user)));
        }
    }
}
