using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;

namespace Backend.Controllers
{
    public class UserController : ControllerBase
    {
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
            public List<User> Get()
            {
                return _userService.GetUsers();
            }

            [HttpPost]

            public User Create([FromBody] User user)
            {
                _userService.CreateUser(user);
                return user;
            }
        }
    }
}