using System.Security.Cryptography;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Thunderbird.Application.Interfaces;
using Thunderbird.Domain.Entities;
using Thunderbird.Domain.Interfaces;

namespace Thunderbird.Application.Services {
    public class CaptchaService : ICaptchaService {
        private const string AllowedChars = "0123456789";
        private const int CodeLength = 4;
        private static readonly TimeSpan CaptchaLifetime = TimeSpan.FromMinutes(5);

        private readonly ICaptchaRepository _captchaRepository;
        private readonly IMemoryCacheProvider _memoryCacheProvider;
        public CaptchaService(ICaptchaRepository captchaRepository, IMemoryCacheProvider memoryCacheProvider) {
            _captchaRepository = captchaRepository;
            _memoryCacheProvider = memoryCacheProvider;
        }

        public async Task<CaptchaInfo> GetCaptcha() {
            string randomCode = GenerateRandomCode();
            long id = await _captchaRepository.Insert(randomCode);

            // The DB row has no expiry/used-once concept, so freshness and single-use
            // are enforced here via a short-lived cache entry keyed by the captcha id.
            _memoryCacheProvider.SetCache(CacheKey(id), randomCode, DateTimeOffset.UtcNow.Add(CaptchaLifetime));

            return new CaptchaInfo {
                Id = id,
                CaptchaCode = randomCode,
                Captcha = GetCaptchaImage(randomCode)
            };
        }

        public async Task<bool> IsValid(long id, string captchaCode) {
            string cacheKey = CacheKey(id);
            string? issuedCode = _memoryCacheProvider.GetFromCache<string>(cacheKey);
            if (issuedCode is null) {
                return false;
            }
            _memoryCacheProvider.ClearCache(cacheKey);

            return await _captchaRepository.IsValid(id, captchaCode);
        }

        private static string CacheKey(long id) => $"captcha:{id}";

        private static string GenerateRandomCode() {
            Span<char> code = stackalloc char[CodeLength];
            for (int i = 0; i < CodeLength; i++) {
                code[i] = AllowedChars[RandomNumberGenerator.GetInt32(AllowedChars.Length)];
            }
            return new string(code);
        }

        private static byte[] GetCaptchaImage(string checkCode) {
            using Image<Rgba32> image = new(checkCode.Length * 20, 30);
            Font font = SystemFonts.CreateFont("Arial", 18, FontStyle.Bold);

            image.Mutate(ctx => {
                ctx.BackgroundColor(Color.AliceBlue);
                ctx.DrawText(checkCode, font, Color.DarkBlue, new PointF(5, 3));
            });

            using MemoryStream memoryStream = new();
            image.SaveAsPng(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
