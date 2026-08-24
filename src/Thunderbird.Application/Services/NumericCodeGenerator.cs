using System.Security.Cryptography;

namespace Thunderbird.Application.Services {
    internal static class NumericCodeGenerator {
        private const string Digits = "0123456789";

        public static string Generate(int length) {
            Span<char> code = stackalloc char[length];
            for (int i = 0; i < length; i++) {
                code[i] = Digits[RandomNumberGenerator.GetInt32(Digits.Length)];
            }
            return new string(code);
        }
    }
}
