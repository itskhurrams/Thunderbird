using Thunderbird.Domain.Entities;
using Thunderbird.Domain.Interfaces;

namespace Thunderbird.Application.Tests.TestDoubles {
    public class FakeUserRepository : IUserRepository {
        public User? UserToReturn { get; set; }
        public (string LoginName, string LoginPassword)? LastLoginCall { get; private set; }

        public Task<User?> Login(string loginName, string loginPassword) {
            LastLoginCall = (loginName, loginPassword);
            return Task.FromResult(UserToReturn);
        }
    }
}
