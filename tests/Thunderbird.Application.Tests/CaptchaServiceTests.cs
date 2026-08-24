using Thunderbird.Application.Services;
using Thunderbird.Application.Tests.TestDoubles;

namespace Thunderbird.Application.Tests {
    public class CaptchaServiceTests {
        [Fact]
        public async Task GetCaptcha_GeneratesFourDigitNumericCode() {
            var repository = new FakeCaptchaRepository();
            var service = new CaptchaService(repository, new FakeMemoryCacheProvider());

            var captcha = await service.GetCaptcha();

            Assert.Equal(4, captcha.CaptchaCode.Length);
            Assert.All(captcha.CaptchaCode, c => Assert.True(char.IsDigit(c)));
            Assert.NotEmpty(captcha.Captcha);
        }

        [Fact]
        public async Task IsValid_ReturnsFalse_WhenCaptchaWasNeverIssued() {
            var service = new CaptchaService(new FakeCaptchaRepository(), new FakeMemoryCacheProvider());

            bool result = await service.IsValid(999, "1234");

            Assert.False(result);
        }

        [Fact]
        public async Task IsValid_ReturnsTrue_ForCorrectCodeOnFirstAttempt() {
            var repository = new FakeCaptchaRepository();
            var service = new CaptchaService(repository, new FakeMemoryCacheProvider());
            var captcha = await service.GetCaptcha();

            bool result = await service.IsValid(captcha.Id, captcha.CaptchaCode);

            Assert.True(result);
        }

        [Fact]
        public async Task IsValid_IsSingleUse_SecondAttemptFailsEvenWithCorrectCode() {
            var repository = new FakeCaptchaRepository();
            var service = new CaptchaService(repository, new FakeMemoryCacheProvider());
            var captcha = await service.GetCaptcha();

            bool first = await service.IsValid(captcha.Id, captcha.CaptchaCode);
            bool second = await service.IsValid(captcha.Id, captcha.CaptchaCode);

            Assert.True(first);
            Assert.False(second);
        }

        [Fact]
        public async Task IsValid_ReturnsFalse_ForWrongCode() {
            var repository = new FakeCaptchaRepository();
            var service = new CaptchaService(repository, new FakeMemoryCacheProvider());
            var captcha = await service.GetCaptcha();

            bool result = await service.IsValid(captcha.Id, "0000".Equals(captcha.CaptchaCode) ? "1111" : "0000");

            Assert.False(result);
        }
    }
}
