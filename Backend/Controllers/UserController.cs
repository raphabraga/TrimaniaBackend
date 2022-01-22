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
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetUsers([FromQuery(Name = "sort")] string sort, [FromQuery(Name = "page")] int? page)
        {
            var users = await _userService.Query(filter: null, sort, page);
            return Ok(users.Select(user => new ViewUser(user)));
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> SearchUsers([FromQuery(Name = "filter")] string filter,
        [FromQuery(Name = "sort")] string sort, [FromQuery(Name = "page")] int? page)
        {
            var users = await _userService.Query(filter, sort, page);
            return Ok(users.Select(user => new ViewUser(user)));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> UserById(int id)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User requestingUser = await _userService.GetUserByLogin(login);
            User user = await _userService.GetUserById(requestingUser, id);
            return Ok(new ViewUser(user));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest newUser)
        {
            User user = new User(newUser);
            await _userService.CreateUser(user);
            return CreatedAtAction(nameof(UserById), new { id = user.Id }, new ViewUser(user));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest userUpdate)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = await _userService.GetUserByLogin(login);
            if (user == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.UserIdNotFound));
            return Ok(new ViewUser(await _userService.UpdateUser(user.Id, userUpdate)));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = await _userService.GetUserByLogin(login);
            if (user == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.UserIdNotFound));
            await _userService.DeleteUser(user.Id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteById(int id)
        {
            await _userService.DeleteUser(id);
            return NoContent();
        }
    }
}