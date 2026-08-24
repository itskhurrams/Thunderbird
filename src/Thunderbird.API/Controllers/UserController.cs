using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

using Thunderbird.API.Models;
using Thunderbird.Application.Interfaces;
using Thunderbird.Domain.Entities;
using Thunderbird.Domain.Models;

namespace Thunderbird.API.Controllers {
    [EnableRateLimiting("auth")]
    public class UserController : BaseController {
        private readonly IUserService _userService;
        private readonly ICaptchaService _captchaService;
        private readonly ITokenService _tokenService;
        private readonly ITwoFactorService _twoFactorService;
        private readonly TokenAuthenticationSettings _tokenSettings;

        public UserController(
            IUserService userService,
            ICaptchaService captchaService,
            ITokenService tokenService,
            ITwoFactorService twoFactorService,
            IOptions<TokenAuthenticationSettings> tokenSettings) {
            _userService = userService;
            _captchaService = captchaService;
            _tokenService = tokenService;
            _twoFactorService = twoFactorService;
            _tokenSettings = tokenSettings.Value;
        }

        [HttpPost]
        public async Task<ActionResult<TwoFactorChallengeResponse>> Login([FromBody] LoginRequest request) {
            bool captchaIsValid = await _captchaService.IsValid(request.CaptchaId, request.CaptchaCode);
            if (!captchaIsValid) {
                return BadRequest("Invalid or expired captcha.");
            }

            var user = await _userService.Login(request.LoginName, request.LoginPassword);
            if (user is null) {
                return Unauthorized();
            }

            return Ok(await IssueTwoFactorChallenge(user));
        }

        [HttpPost("register")]
        public async Task<ActionResult<TwoFactorChallengeResponse>> Register([FromBody] RegisterRequest request) {
            bool captchaIsValid = await _captchaService.IsValid(request.CaptchaId, request.CaptchaCode);
            if (!captchaIsValid) {
                return BadRequest("Invalid or expired captcha.");
            }

            var result = await _userService.Register(request.LoginName, request.Password, request.FirstName, request.LastName, request.Email, request.PhoneNumber);
            if (!result.Succeeded) {
                return Conflict(result.Error);
            }

            return Ok(await IssueTwoFactorChallenge(result.User!));
        }

        [HttpPost("2fa/verify")]
        public async Task<ActionResult<AuthResponse>> VerifyTwoFactor([FromBody] TwoFactorVerifyRequest request) {
            var result = await _twoFactorService.Verify(request.ChallengeId, request.Code);
            if (!result.Succeeded) {
                return Unauthorized(result.Error);
            }

            return Ok(IssueAuthResponse(result.User!));
        }

        private async Task<TwoFactorChallengeResponse> IssueTwoFactorChallenge(User user) {
            string challengeId = await _twoFactorService.IssueChallenge(user);
            return new TwoFactorChallengeResponse(challengeId, "A verification code has been sent to your email and phone.");
        }

        private AuthResponse IssueAuthResponse(User user) {
            string token = _tokenService.GenerateToken(user);
            Response.Cookies.Append(_tokenSettings.CookieName, token, new CookieOptions {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(_tokenSettings.ExpiryMinutes)
            });
            return new AuthResponse(token, UserResponse.FromUser(user));
        }
    }
}
