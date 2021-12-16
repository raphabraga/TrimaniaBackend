using System.Net;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("orders")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly UserService _userService;

        public OrderController(OrderService oService, UserService uService)
        {
            _orderService = oService;
            _userService = uService;
        }

        [HttpGet]
        public IActionResult GetUserOrders()
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            return Ok(_orderService.GetOrders(user));
        }

        [HttpPost]
        public IActionResult AddProductToChart([FromBody] Product product)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User loggedUser = _userService.GetUserByLogin(login);
            string role = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role).Value;
            return Ok(_orderService.AddToChart(loggedUser, product));
        }
    }
}