using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Backend.Models.ViewModels;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Backend.Models.Exceptions;
using Backend.Models.Enums;
using Backend.Utils;

namespace Backend.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService service)
        {
            _userService = service;
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult GetUsers([FromQuery(Name = "filter")] string filter,
        [FromQuery(Name = "sort")] string sort, [FromQuery(Name = "page")] int? page)
        {
            try
            {
                return Ok(_userService.Query(filter, sort, page).Select(user => new ViewUser(user)));
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public IActionResult UserById(int id)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            try
            {
                User requestingUser = _userService.GetUserByLogin(login);
                User user = _userService.GetUserById(requestingUser, id);
                return Ok(new ViewUser(user));
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (RegisterNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
                return NotFound(e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(e.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult NewUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                _userService.CreateUser(user);
                return CreatedAtAction(nameof(UserById), new { id = user.Id }, new ViewUser(user));
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (AggregateException e)
            {
                System.Console.WriteLine(e.Message);
                return UnprocessableEntity(e.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateUser([FromBody] UpdateUser userUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            try
            {
                User user = _userService.GetUserByLogin(login);
                if (user == null)
                    throw new RegisterNotFoundException(ErrorMessage.GetMessage(ErrorType.UserIdNotFound));
                return Ok(new ViewUser(_userService.UpdateUser(user.Id, userUpdate)));
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (RegisterNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
                return NotFound(e.Message);
            }
        }

        [HttpDelete]
        public IActionResult DeleteUser()
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            try
            {
                User user = _userService.GetUserByLogin(login);
                if (user == null)
                    throw new RegisterNotFoundException(ErrorMessage.GetMessage(ErrorType.UserIdNotFound));
                _userService.DeleteUser(user.Id);
                return NoContent();
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (NotAllowedDeletionException e)
            {
                System.Console.WriteLine(e.Message);
                return UnprocessableEntity(e.Message);
            }
            catch (RegisterNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
                return NotFound(e.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteById(int id)
        {
            try
            {
                _userService.DeleteUser(id);
                return NoContent();
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (NotAllowedDeletionException e)
            {
                System.Console.WriteLine(e.Message);
                return UnprocessableEntity(e.Message);
            }
            catch (RegisterNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
                return NotFound(e.Message);
            }
        }
    }
}