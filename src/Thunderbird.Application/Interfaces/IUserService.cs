using Thunderbird.Domain.Entities;

namespace Thunderbird.Application.Interfaces {
    public interface IUserService {
        public Task<User> Login(string loginName, string loginPassword);
    }
}
