namespace Thunderbird.Domain.Interfaces {
    public interface ICaptchaRepository {

        public Task<bool> IsValid(long Id, string CaptchCode);
        public Task<long> Insert(string CaptchCode);

    }
}
