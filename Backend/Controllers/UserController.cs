using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Backend.Interfaces.Services;
using Backend.Models.Exceptions;
using Backend.Models.Enums;
using Backend.Utils;
using Backend.ViewModels;
using Backend.Dtos;

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
        public IActionResult GetUsers([FromQuery(Name = "sort")] string sort, [FromQuery(Name = "page")] int? page)
        {
            return Ok(_userService.Query(filter: null, sort, page).Select(user => new ViewUser(user)));
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        [Route("search")]
        public IActionResult SearchUsers([FromQuery(Name = "filter")] string filter,
        [FromQuery(Name = "sort")] string sort, [FromQuery(Name = "page")] int? page)
        {
            return Ok(_userService.Query(filter, sort, page).Select(user => new ViewUser(user)));
        }

        [HttpGet("{id}")]
        public IActionResult UserById(int id)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User requestingUser = _userService.GetUserByLogin(login);
            User user = _userService.GetUserById(requestingUser, id);
            return Ok(new ViewUser(user));
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult NewUser([FromBody] User user)
        {
            _userService.CreateUser(user);
            return CreatedAtAction(nameof(UserById), new { id = user.Id }, new ViewUser(user));
        }

        [HttpPut]
        public IActionResult UpdateUser([FromBody] UpdateUser userUpdate)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            if (user == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.UserIdNotFound));
            return Ok(new ViewUser(_userService.UpdateUser(user.Id, userUpdate)));
        }

        [HttpDelete]
        public IActionResult DeleteUser()
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            if (user == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.UserIdNotFound));
            _userService.DeleteUser(user.Id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteById(int id)
        {
            _userService.DeleteUser(id);
            return NoContent();
        }
    }
}