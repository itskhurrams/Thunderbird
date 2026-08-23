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

        private readonly ICaptchaRepository _captchaRepository;
        public CaptchaService(ICaptchaRepository captchaRepository) {
            _captchaRepository = captchaRepository;
        }

        public async Task<CaptchaInfo> GetCaptcha() {
            string randomCode = GenerateRandomCode();

            CaptchaInfo captchaInfo = new() {
                Id = await _captchaRepository.Insert(randomCode),
                CaptchaCode = randomCode,
                Captcha = GetCaptchaImage(randomCode)
            };
            return captchaInfo;
        }

        public async Task<bool> IsValid(long id, string captchaCode) {
            return await _captchaRepository.IsValid(id, captchaCode);
        }

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
