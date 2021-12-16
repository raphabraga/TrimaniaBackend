using System.Linq;
using System.Security.Claims;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        public UsersController(UserService service)
        {
            _userService = service;
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult Get([FromQuery(Name = "filter")] string filter,
        [FromQuery(Name = "sort")] string sort, [FromQuery(Name = "page")] int page)
        {
            return Ok(_userService.Query(filter, sort, page));
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            string role = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role).Value;
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserById(id);
            if (role == "Administrator" || user.Login == login)
                return Ok(user);
            else
                return Unauthorized("Operation forbidden for this credentials");
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Create([FromBody] User user)
        {
            _userService.CreateUser(user);
            return Ok("User successfully created on database\n\n" + user);
        }

        [HttpPut]
        public IActionResult Update([FromBody] User userUpdate)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);

            if (_userService.UpdateUser(user.Id, userUpdate))
                return Ok("User successfully updated on database\n\n" + user);
            else
                return BadRequest("No user with this ID on the database.");
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);

            if (_userService.DeleteUser(user.Id))
                return Ok("User successfully deleted from database");
            else
                return BadRequest("No user with this ID on the database.");
        }
    }
}
