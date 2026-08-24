using Thunderbird.Application.Interfaces;
using Thunderbird.Application.Models;
using Thunderbird.Domain.Entities;
using Thunderbird.Domain.Interfaces;

namespace Thunderbird.Application.Services {
    public class UserService : IUserService {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher) {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<User?> Login(string loginName, string loginPassword) {
            User? user = await _userRepository.GetByLoginName(loginName);
            if (user is null) {
                return null;
            }

            if (_passwordHasher.IsHashed(user.LoginPassword)) {
                return _passwordHasher.Verify(user.LoginPassword, loginPassword) ? user : null;
            }

            // Legacy account created before password hashing was introduced: the stored value
            // is still plaintext. Accept it once, then transparently upgrade it to a hash.
            if (user.LoginPassword != loginPassword) {
                return null;
            }
            await _userRepository.UpdatePassword(user.UserId, _passwordHasher.Hash(loginPassword));
            return user;
        }

        public async Task<RegisterResult> Register(string loginName, string password, string firstName, string lastName, string email, string phoneNumber) {
            string hashedPassword = _passwordHasher.Hash(password);
            long userId = await _userRepository.Register(loginName, hashedPassword, firstName, lastName, email, phoneNumber);
            if (userId <= 0) {
                return RegisterResult.DuplicateLoginName();
            }

            return RegisterResult.Success(new User {
                UserId = userId,
                LoginName = loginName,
                LoginPassword = string.Empty,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                IsActive = true
            });
        }
    }
}
