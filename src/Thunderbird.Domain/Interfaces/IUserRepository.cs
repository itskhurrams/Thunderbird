using Thunderbird.Domain.Entities;
namespace Thunderbird.Domain.Interfaces {
    public interface IUserRepository {
        public Task<User?> GetByLoginName(string loginName);
        public Task UpdatePassword(long userId, string hashedPassword);
        public Task<long> Register(string loginName, string hashedPassword, string firstName, string lastName, string email, string phoneNumber);
    }
}
