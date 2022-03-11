using Microsoft.AspNetCore.Mvc;

using Thunderbird.Application.Interfaces;
using Thunderbird.Domain.Entities;

namespace Thunderbird.API.Controllers {
    public class UserController : BaseController {
        private readonly IUserService _userService;
        public UserController(IUserService userService) {
            _userService = userService;
        }
       [HttpPost]
        public async Task<User> Login(string loginName , string loginPassword) {
            return await _userService.Login(loginName, loginPassword);
        }
    }
}
