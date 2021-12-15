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
        public List<User> Get([FromQuery(Name = "query")] string query,
        [FromQuery(Name = "sort")] string sort, [FromQuery(Name = "page")] int page)
        {
            return _userService.Query(query, sort, page);
        }

        [HttpGet("{id}")]
        public User Get(int id)
        {
            return _userService.GetUserById(id);
        }

        [HttpPost]
        [AllowAnonymous]
        public User Create([FromBody] User user)
        {
            _userService.CreateUser(user);
            return user;
        }

    }
}
