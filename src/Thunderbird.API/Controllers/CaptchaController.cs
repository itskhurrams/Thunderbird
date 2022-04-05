using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Thunderbird.Application.Interfaces;
using Thunderbird.Domain.Entities;

namespace Thunderbird.API.Controllers {
    public class CaptchaController : BaseController {
        private readonly ICaptchaService  _captchaService;
        public CaptchaController(ICaptchaService captchaService) {
            _captchaService = captchaService;
        }
        [HttpGet]
        public async Task<CaptchaInfo> GetCaptcha() {
            return await _captchaService.GetCaptcha();
        }
    }

}
