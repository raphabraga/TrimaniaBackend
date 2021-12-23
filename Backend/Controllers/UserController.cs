using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Backend.Models.ViewModels;
using Backend.Interfaces;

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
            if (role != "Administrator" && user?.Login != login)
                return Unauthorized("Credentials not allowed for the operation.");
            if (user == null)
                return NotFound("No user registered on the database with this ID");
            return Ok(new ViewUser(user));

        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult NewUser([FromBody] User userInfo)
        {
            if (!ModelState.IsValid)
                return BadRequest("JSON object provided is formatted wrong.");
            User user = _userService.Query(userInfo.Login, null, null).FirstOrDefault();
            if (user != null)
                return Conflict("User already registered on the database with this login.");
            user = _userService.Query(userInfo.Email, null, null).FirstOrDefault();
            if (user != null)
                return Conflict("User already registered on the database with this email.");
            if (_userService.CreateUser(userInfo) == null)
                return BadRequest("Required fields for user register not filled.");
            return CreatedAtAction(nameof(UserById), new { id = userInfo.Id }, new ViewUser(userInfo));
        }

        [HttpPut]
        public IActionResult UpdateUser([FromBody] UpdateUser userUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest("JSON object provided is formatted wrong.");
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            if (user == null)
                return NotFound("No user registered on the database with this ID.");
            _userService.UpdateUser(user.Id, userUpdate);
            return Ok(new ViewUser(user));
        }

        [HttpDelete]
        public IActionResult DeleteUser()
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            if (user == null)
                return NotFound("No user registered on the database with this ID.");
            if (_userService.DeleteUser(user.Id))
                return NoContent();
            else
                return UnprocessableEntity("User has registered orders, deletion is forbidden");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteById(int id)
        {
            User user = _userService.GetUserById(id);
            if (user == null)
                return NotFound("No user registered on the database with this ID.");
            if (_userService.DeleteUser(user.Id))
                return NoContent();
            else
                return UnprocessableEntity("User has registered orders, deletion is forbidden");
        }
    }
}
