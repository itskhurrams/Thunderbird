using Thunderbird.Domain.Entities;
namespace Thunderbird.Domain.Interfaces {
    public interface IUserRepository {
        public Task<User> Login(string loginName, string loginPassword);
    }
}
