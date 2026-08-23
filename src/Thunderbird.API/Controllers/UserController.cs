using Microsoft.AspNetCore.Mvc;

using Thunderbird.API.Models;
using Thunderbird.Application.Interfaces;

namespace Thunderbird.API.Controllers {
    public class UserController : BaseController {
        private readonly IUserService _userService;
        public UserController(IUserService userService) {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<UserResponse>> Login([FromBody] LoginRequest request) {
            var user = await _userService.Login(request.LoginName, request.LoginPassword);
            if (user is null) {
                return Unauthorized();
            }
            return Ok(UserResponse.FromUser(user));
        }
    }
}
