using Thunderbird.Domain.Entities;

namespace Thunderbird.API.Models {
    public record CaptchaResponse(long Id, byte[] Captcha) {
        public static CaptchaResponse FromCaptchaInfo(CaptchaInfo captchaInfo) =>
            new(captchaInfo.Id, captchaInfo.Captcha);
    }
}
