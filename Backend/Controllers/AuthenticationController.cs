using System.Net;
using System;
using Microsoft.AspNetCore.Mvc;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Backend.Dtos;
using Backend.Models.ViewModels;

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
            try
            {
                string token = _userService.GetAuthenticationToken(authUser);
                return Ok(new { token });
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status502BadGateway, new ErrorMessage(e, HttpStatusCode.BadGateway));
            }
            catch (UnauthorizedAccessException e)
            {
                System.Console.WriteLine(e.Message);
                return Unauthorized(new ErrorMessage(e, System.Net.HttpStatusCode.Unauthorized));
            }
        }
    }
}