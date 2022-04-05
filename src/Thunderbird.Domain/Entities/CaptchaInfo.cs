namespace Thunderbird.Domain.Entities {
    public class CaptchaInfo {
        public long Id { get; set; }
        public byte[] Captcha { get; set; }
        public string CaptchaCode { get; set; }
    }
}
