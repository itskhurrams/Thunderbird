namespace Thunderbird.Domain.Entities {
    public class CaptchaInfo {
        public long Id { get; set; }
        public required byte[] Captcha { get; set; }
        public required string CaptchaCode { get; set; }
    }
}
