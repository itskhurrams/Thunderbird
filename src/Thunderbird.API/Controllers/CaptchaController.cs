using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Thunderbird.API.Models;
using Thunderbird.Application.Interfaces;

namespace Thunderbird.API.Controllers {
    [EnableRateLimiting("auth")]
    public class CaptchaController : BaseController {
        private readonly ICaptchaService _captchaService;
        public CaptchaController(ICaptchaService captchaService) {
            _captchaService = captchaService;
        }

        [HttpGet]
        public async Task<CaptchaResponse> GetCaptcha() {
            var captchaInfo = await _captchaService.GetCaptcha();
            return CaptchaResponse.FromCaptchaInfo(captchaInfo);
        }
    }
}
