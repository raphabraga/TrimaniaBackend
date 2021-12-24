using System;
using Backend.Models;
using Backend.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Backend.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Backend.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("/api/v{version:apiVersion}/login")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthenticationController(IUserService uService, ITokenService tService)
        {
            _userService = uService;
            _tokenService = tService;
        }

        [HttpPost]
        public IActionResult Login(AuthUser authUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                User user = _userService.GetUserByLogin(authUser.Login);
                if (user == null)
                    return Unauthorized("Incorrect login and/or password.");
                if (!_userService.CheckPassword(user, authUser.Password))
                    return Unauthorized("Incorrect login and/or password.");
                string token = _tokenService.GenerateToken(user);
                return Ok(new { token });
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}