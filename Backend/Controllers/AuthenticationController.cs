using Backend.Models;
using Backend.Services;
using Backend.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthenticationController(UserService service)
        {
            _userService = service;
        }

        [HttpPost]
        public IActionResult Login(AuthUser authUser)
        {
            if (!_userService.CheckPassword(authUser.Login, authUser.Password))
                return BadRequest("Invalid login or password");
            User user = _userService.GetUserByLogin(authUser.Login);
            string token = TokenService.GenerateToken(user);
            return Ok(token);
        }
    }
}