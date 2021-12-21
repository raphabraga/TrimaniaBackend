using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Backend.Models.ViewModels;

namespace Backend.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        public UsersController(UserService service)
        {
            _userService = service;
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult AllUsers([FromQuery(Name = "filter")] string filter,
        [FromQuery(Name = "sort")] string sort, [FromQuery(Name = "page")] int page)
        {
            return Ok(_userService.Query(filter, sort, page).Select(user => new ViewUser(user)));
        }

        [HttpGet("{id}")]
        public IActionResult UserById(int id)
        {
            string role = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role).Value;
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserById(id);
            if (role == "Administrator" || user.Login == login)
                return Ok(new ViewUser(user));
            else
                return Unauthorized("Credentials not allowed for the operation.");
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult NewUser([FromBody] User userInfo)
        {
            if (!ModelState.IsValid)
                return BadRequest("JSON object provided is formatted wrong.");
            User user = _userService.Query(userInfo.Login, null, null).FirstOrDefault();
            if (user != null)
                return BadRequest("User already registered on the database with this login.");
            user = _userService.Query(userInfo.Email, null, null).FirstOrDefault();
            if (user != null)
                return BadRequest("User already registered on the database with this email.");
            _userService.CreateUser(userInfo);
            return Ok(new ViewUser(userInfo));
        }

        [HttpPut]
        public IActionResult UpdateUser([FromBody] UpdateUser userUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest("JSON object provided is formatted wrong.");
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            if (user == null)
                return NotFound("User not registered on the database.");
            _userService.UpdateUser(user.Id, userUpdate);
            return Ok(new ViewUser(user));
        }

        [HttpDelete]
        public IActionResult DeleteUser()
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            if (user == null)
                return NotFound("User not registered on the database.");
            if (_userService.DeleteUser(user.Id))
                return NoContent();
            else
                return BadRequest("User has registered orders, deletion is forbidden");
        }
    }
}
