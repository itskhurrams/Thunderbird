using System.Drawing;
using Thunderbird.Application.Interfaces;
using Thunderbird.Domain.Entities;
using Thunderbird.Domain.Interfaces;

namespace Thunderbird.Application.Services {
    public class CaptchaService : ICaptchaService {
        private readonly ICaptchaRepository _captcharepository;
        public CaptchaService(ICaptchaRepository captchaRepository) {
            _captcharepository = captchaRepository;
        }
        public async Task<CaptchaInfo> GetCaptcha() {
            string allChar = "0,1,2,3,4,5,6,7,8,9";
            string[] allCharArray = allChar.Split(',');
            string randomCode = "";
            int temporary = -1;
            Random rand = new Random();
            for (int i = 0; i < 4; i++) {
                if (temporary != -1) {
                    rand = new Random(i * temporary * ((int)DateTime.Now.Ticks));
                }
                int nextRendom = rand.Next(10);
                temporary = nextRendom;
                randomCode += allCharArray[nextRendom];
            }

            CaptchaInfo captchaInfo = new CaptchaInfo {
                Id = await _captcharepository.Insert(randomCode),
                CaptchaCode = randomCode,
                Captcha = GetCaptchaImage(randomCode)
            };
            return captchaInfo;
        }

        public async Task<bool> IsValid(long Id, string CaptchaCode) {
            return await _captcharepository.IsValid(Id, CaptchaCode);
        }
        private byte[] GetCaptchaImage(string checkCode) {
            
            using Bitmap bitmap = new(Convert.ToInt32(Math.Ceiling((decimal)(checkCode.Length * 15))), 25);
            using Graphics graphics = Graphics.FromImage(bitmap);
            Random random = new();
            graphics.Clear(Color.AliceBlue);
            Font font = new Font("Comic Sans MS", 14, FontStyle.Strikeout);
            string codeString = "";
            System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, bitmap.Width, bitmap.Height), Color.Blue, Color.DarkRed, 1.2f, true);
            for (int i = 0; i < checkCode.Length; i++) {
                codeString += checkCode.Substring(i, 1);
            }
            graphics.DrawString(codeString, font, new SolidBrush(Color.Blue), 0, 0);
            graphics.Flush();
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Gif);
            return memoryStream.ToArray();
        }
    }
}
