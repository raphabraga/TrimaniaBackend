using Backend.Models;
using Backend.Services;
using Backend.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("/api/v{version:apiVersion}/login")]
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
            if (!ModelState.IsValid)
                return BadRequest("Data provided in the wrong format.");
            User user = _userService.GetUserByLogin(authUser.Login);
            if (user == null)
                return NotFound("User not registered on the database.");
            if (!_userService.CheckPassword(user, authUser.Password))
                return BadRequest("Incorrect login and/or password.");
            string token = TokenService.GenerateToken(user);
            return Ok("Authentication token: " + token);
        }
    }
}