using Thunderbird.Domain.Entities;

namespace Thunderbird.Application.Models {
    public record RegisterResult(bool Succeeded, string? Error, User? User) {
        public static RegisterResult Success(User user) => new(true, null, user);
        public static RegisterResult DuplicateLoginName() => new(false, "A user with this login name already exists.", null);
    }
}
