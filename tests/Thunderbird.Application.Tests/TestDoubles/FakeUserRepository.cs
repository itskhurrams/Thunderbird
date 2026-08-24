using Thunderbird.Domain.Entities;
using Thunderbird.Domain.Interfaces;

namespace Thunderbird.Application.Tests.TestDoubles {
    public class FakeUserRepository : IUserRepository {
        private readonly Dictionary<string, User> _usersByLoginName = new();
        private long _nextId = 1;

        public string? LastLoginNameLookup { get; private set; }
        public (long UserId, string HashedPassword)? LastUpdatePasswordCall { get; private set; }

        public void Seed(User user) => _usersByLoginName[user.LoginName] = user;

        public Task<User?> GetByLoginName(string loginName) {
            LastLoginNameLookup = loginName;
            return Task.FromResult(_usersByLoginName.TryGetValue(loginName, out var user) ? user : null);
        }

        public Task UpdatePassword(long userId, string hashedPassword) {
            LastUpdatePasswordCall = (userId, hashedPassword);
            var user = _usersByLoginName.Values.FirstOrDefault(u => u.UserId == userId);
            if (user is not null) {
                user.LoginPassword = hashedPassword;
            }
            return Task.CompletedTask;
        }

        public Task<long> Register(string loginName, string hashedPassword, string firstName, string lastName, string email, string phoneNumber) {
            if (_usersByLoginName.ContainsKey(loginName)) {
                return Task.FromResult(-1L);
            }

            long id = _nextId++;
            _usersByLoginName[loginName] = new User {
                UserId = id,
                LoginName = loginName,
                LoginPassword = hashedPassword,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                IsActive = true
            };
            return Task.FromResult(id);
        }
    }
}
