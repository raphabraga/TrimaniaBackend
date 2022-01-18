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
        public AuthenticationController(IUserService uService)
        {
            _userService = uService;
        }

        [HttpPost]
        public IActionResult Login(AuthUser authUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            string token = _userService.GetAuthenticationToken(authUser);
            return Ok(new { token });
        }
    }
}