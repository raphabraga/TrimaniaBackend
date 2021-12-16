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

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get([FromQuery(Name = "query")] string query,
        [FromQuery(Name = "sort")] string sort, [FromQuery(Name = "page")] int page)
        {
            return Ok(_userService.Query(query, sort, page));
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok(_userService.GetUserById(id));
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Create([FromBody] User user)
        {
            _userService.CreateUser(user);
            return Ok("User successfully created on database\n\n" + user);
        }

        [HttpPut("{id}")]
        [AllowAnonymous]

        public IActionResult Update(int id, [FromBody] User user)
        {
            if (_userService.UpdateUser(id, user))
                return Ok("User successfully updated on database\n\n" + user);
            else
                return BadRequest("No user with this ID on the database.");
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]

        public IActionResult Delete(int id)
        {
            if (_userService.DeleteUser(id))
                return Ok("User successfully deleted from database");
            else
                return BadRequest("No user with this ID on the database.");
        }
    }
}
