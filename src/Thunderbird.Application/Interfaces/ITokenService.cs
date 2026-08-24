using Thunderbird.Domain.Entities;

namespace Thunderbird.Application.Interfaces {
    public interface ITokenService {
        public string GenerateToken(User user);
    }
}
