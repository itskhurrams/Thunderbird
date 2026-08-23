using Microsoft.AspNetCore.Mvc;
using Thunderbird.API.Models;
using Thunderbird.Application.Interfaces;

namespace Thunderbird.API.Controllers {
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

        [HttpPost("validate")]
        public async Task<ActionResult<bool>> Validate(CaptchaValidationRequest request) {
            bool isValid = await _captchaService.IsValid(request.Id, request.CaptchaCode);
            return isValid ? Ok(true) : Unauthorized(false);
        }
    }
}
