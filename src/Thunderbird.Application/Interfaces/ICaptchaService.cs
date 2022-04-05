using Thunderbird.Domain.Entities;

namespace Thunderbird.Application.Interfaces {
    public interface ICaptchaService {
        public Task<CaptchaInfo> GetCaptcha();
        public Task<bool> IsValid(long Id , string CaptchaCode);

    }
}
