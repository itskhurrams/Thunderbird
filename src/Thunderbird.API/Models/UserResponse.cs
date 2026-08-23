using Thunderbird.Domain.Entities;

namespace Thunderbird.API.Models {
    public record UserResponse(long UserId, string LoginName, string FirstName, string LastName, bool IsActive) {
        public static UserResponse FromUser(User user) =>
            new(user.UserId, user.LoginName, user.FirstName, user.LastName, user.IsActive);
    }
}
