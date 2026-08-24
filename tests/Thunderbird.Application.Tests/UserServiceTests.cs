using Thunderbird.Application.Services;
using Thunderbird.Application.Tests.TestDoubles;
using Thunderbird.Domain.Entities;

namespace Thunderbird.Application.Tests {
    public class UserServiceTests {
        private static UserService CreateService(FakeUserRepository repository) =>
            new(repository, new PasswordHasher());

        private static User SeededUser(FakeUserRepository repository, string password) {
            var user = new User {
                UserId = 1, LoginName = "jdoe", LoginPassword = password,
                FirstName = "John", LastName = "Doe",
                Email = "jdoe@example.com", PhoneNumber = "+12025550123"
            };
            repository.Seed(user);
            return user;
        }

        [Fact]
        public async Task Login_ReturnsNull_WhenNoUserWithThatLoginName() {
            var service = CreateService(new FakeUserRepository());

            var result = await service.Login("nobody", "whatever");

            Assert.Null(result);
        }

        [Fact]
        public async Task Login_Succeeds_WithCorrectHashedPassword() {
            var hasher = new PasswordHasher();
            var repository = new FakeUserRepository();
            SeededUser(repository, hasher.Hash("correct-password"));
            var service = CreateService(repository);

            var result = await service.Login("jdoe", "correct-password");

            Assert.NotNull(result);
            Assert.Equal(1, result!.UserId);
        }

        [Fact]
        public async Task Login_Fails_WithWrongHashedPassword() {
            var hasher = new PasswordHasher();
            var repository = new FakeUserRepository();
            SeededUser(repository, hasher.Hash("correct-password"));
            var service = CreateService(repository);

            var result = await service.Login("jdoe", "wrong-password");

            Assert.Null(result);
        }

        [Fact]
        public async Task Login_AcceptsLegacyPlaintextPassword_AndUpgradesItToAHash() {
            var repository = new FakeUserRepository();
            SeededUser(repository, "still-plaintext");
            var service = CreateService(repository);

            var result = await service.Login("jdoe", "still-plaintext");

            Assert.NotNull(result);
            Assert.NotNull(repository.LastUpdatePasswordCall);
            Assert.Equal(1, repository.LastUpdatePasswordCall!.Value.UserId);
            Assert.NotEqual("still-plaintext", repository.LastUpdatePasswordCall.Value.HashedPassword);

            // The upgraded hash must itself verify against the original password.
            var loginAfterUpgrade = await service.Login("jdoe", "still-plaintext");
            Assert.NotNull(loginAfterUpgrade);
        }

        [Fact]
        public async Task Login_RejectsWrongLegacyPlaintextPassword() {
            var repository = new FakeUserRepository();
            SeededUser(repository, "still-plaintext");
            var service = CreateService(repository);

            var result = await service.Login("jdoe", "wrong-guess");

            Assert.Null(result);
            Assert.Null(repository.LastUpdatePasswordCall);
        }

        [Fact]
        public async Task Register_Succeeds_ForNewLoginName() {
            var service = CreateService(new FakeUserRepository());

            var result = await service.Register("jdoe", "some-password", "John", "Doe", "jdoe@example.com", "+12025550123");

            Assert.True(result.Succeeded);
            Assert.NotNull(result.User);
            Assert.Equal("jdoe", result.User!.LoginName);
            Assert.Equal("jdoe@example.com", result.User.Email);
            Assert.Equal("+12025550123", result.User.PhoneNumber);
        }

        [Fact]
        public async Task Register_Fails_ForDuplicateLoginName() {
            var repository = new FakeUserRepository();
            var service = CreateService(repository);
            await service.Register("jdoe", "some-password", "John", "Doe", "jdoe@example.com", "+12025550123");

            var result = await service.Register("jdoe", "different-password", "Jane", "Doe", "jane@example.com", "+12025550124");

            Assert.False(result.Succeeded);
            Assert.Null(result.User);
            Assert.NotNull(result.Error);
        }

        [Fact]
        public async Task Register_StoresAHashedPassword_NotThePlaintext() {
            var repository = new FakeUserRepository();
            var service = CreateService(repository);

            await service.Register("jdoe", "some-password", "John", "Doe", "jdoe@example.com", "+12025550123");
            var stored = await repository.GetByLoginName("jdoe");

            Assert.NotNull(stored);
            Assert.NotEqual("some-password", stored!.LoginPassword);
        }
    }
}
