using System;
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
        private readonly TokenService _tokenService;

        public AuthenticationController(UserService uService, TokenService tService)
        {
            _userService = uService;
            _tokenService = tService;
        }

        [HttpPost]
        public IActionResult Login(AuthUser authUser)
        {
            if (!ModelState.IsValid)
                return BadRequest("JSON object provided is formatted wrong.");
            User user = _userService.GetUserByLogin(authUser.Login);
            if (user == null)
                return NotFound("User not registered on the database.");
            if (!_userService.CheckPassword(user, authUser.Password))
                return Unauthorized("Incorrect login and/or password.");
            string token = _tokenService.GenerateToken(user);
            return Ok(new { token });
        }
    }
}