using Thunderbird.Application.Models;
using Thunderbird.Domain.Entities;

namespace Thunderbird.Application.Interfaces {
    public interface IUserService {
        public Task<User?> Login(string loginName, string loginPassword);
        public Task<RegisterResult> Register(string loginName, string password, string firstName, string lastName, string email, string phoneNumber);
    }
}
