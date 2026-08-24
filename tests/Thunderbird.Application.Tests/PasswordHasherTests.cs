using Thunderbird.Application.Services;

namespace Thunderbird.Application.Tests {
    public class PasswordHasherTests {
        [Fact]
        public void Hash_ThenVerify_WithCorrectPassword_Succeeds() {
            var hasher = new PasswordHasher();
            string hash = hasher.Hash("correct-password");

            Assert.True(hasher.Verify(hash, "correct-password"));
        }

        [Fact]
        public void Verify_WithWrongPassword_Fails() {
            var hasher = new PasswordHasher();
            string hash = hasher.Hash("correct-password");

            Assert.False(hasher.Verify(hash, "wrong-password"));
        }

        [Fact]
        public void Hash_ProducesADifferentValue_EachTime() {
            var hasher = new PasswordHasher();

            string first = hasher.Hash("same-password");
            string second = hasher.Hash("same-password");

            Assert.NotEqual(first, second);
        }

        [Fact]
        public void IsHashed_ReturnsTrue_ForOwnHashFormat() {
            var hasher = new PasswordHasher();
            string hash = hasher.Hash("correct-password");

            Assert.True(hasher.IsHashed(hash));
        }

        [Theory]
        [InlineData("plain-text-password")]
        [InlineData("")]
        [InlineData("100000.not-base64.also-not-base64")]
        public void IsHashed_ReturnsFalse_ForNonHashValues(string value) {
            var hasher = new PasswordHasher();

            Assert.False(hasher.IsHashed(value));
        }

        [Fact]
        public void Verify_ReturnsFalse_ForLegacyPlaintextValue() {
            var hasher = new PasswordHasher();

            Assert.False(hasher.Verify("plaintext-legacy-password", "plaintext-legacy-password"));
        }
    }
}
