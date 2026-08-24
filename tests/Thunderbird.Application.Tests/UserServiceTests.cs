using Thunderbird.Application.Services;
using Thunderbird.Application.Tests.TestDoubles;
using Thunderbird.Domain.Entities;

namespace Thunderbird.Application.Tests {
    public class UserServiceTests {
        [Fact]
        public async Task Login_ReturnsNull_WhenRepositoryFindsNoUser() {
            var repository = new FakeUserRepository { UserToReturn = null };
            var service = new UserService(repository);

            var result = await service.Login("nobody", "wrong");

            Assert.Null(result);
        }

        [Fact]
        public async Task Login_ReturnsUser_WhenRepositoryFindsAMatch() {
            var user = new User {
                UserId = 1,
                LoginName = "jdoe",
                LoginPassword = string.Empty,
                FirstName = "John",
                LastName = "Doe"
            };
            var repository = new FakeUserRepository { UserToReturn = user };
            var service = new UserService(repository);

            var result = await service.Login("jdoe", "correct-password");

            Assert.Same(user, result);
            Assert.Equal(("jdoe", "correct-password"), repository.LastLoginCall);
        }
    }
}
