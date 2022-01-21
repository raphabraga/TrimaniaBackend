using Microsoft.AspNetCore.Mvc;
using Backend.Interfaces.Services;
using Backend.Dtos;

namespace Backend.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("/api/v{version:apiVersion}/login")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthenticationController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public IActionResult Login(AuthenticationRequest authenticationRequest)
        {
            string token = _userService.GetAuthenticationToken(authenticationRequest);
            return Ok(new { token });
        }
    }
}