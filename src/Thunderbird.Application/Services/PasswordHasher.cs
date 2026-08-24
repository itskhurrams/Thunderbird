using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Thunderbird.Application.Interfaces;

namespace Thunderbird.Application.Services {
    public class PasswordHasher : IPasswordHasher {
        private const int SaltSizeBytes = 16;
        private const int KeySizeBytes = 32;
        private const int Iterations = 100_000;
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

        // Format: {iterations}.{base64 salt}.{base64 key} - distinguishable at a glance from a
        // legacy plaintext password stored before hashing was introduced (see IsHashed).
        public string Hash(string password) {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
            byte[] key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySizeBytes);
            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        public bool Verify(string hashedPassword, string providedPassword) {
            if (!TryParse(hashedPassword, out int iterations, out byte[]? salt, out byte[]? key)) {
                return false;
            }

            byte[] providedKey = Rfc2898DeriveBytes.Pbkdf2(providedPassword, salt, iterations, Algorithm, key.Length);
            return CryptographicOperations.FixedTimeEquals(key, providedKey);
        }

        public bool IsHashed(string value) => TryParse(value, out _, out _, out _);

        private static bool TryParse(string value, out int iterations, [NotNullWhen(true)] out byte[]? salt, [NotNullWhen(true)] out byte[]? key) {
            iterations = 0;
            salt = null;
            key = null;

            string[] parts = value.Split('.', 3);
            if (parts.Length != 3 || !int.TryParse(parts[0], out iterations)) {
                return false;
            }

            try {
                salt = Convert.FromBase64String(parts[1]);
                key = Convert.FromBase64String(parts[2]);
                return true;
            }
            catch (FormatException) {
                return false;
            }
        }
    }
}
