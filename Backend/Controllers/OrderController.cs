using System;
using System.Net;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.ViewModel;

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
            User user = _userService.GetUserByLogin(login);
            return Ok(_orderService.AddToChart(user, product));
        }

        [HttpPut]
        [Route("cancel")]
        public IActionResult Cancel()
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);

            if (_orderService.CancelOrder(user))
                return Ok("The order was successfully cancelled");
            else
                return BadRequest("Unable to cancel this order");
        }

        [HttpPut]
        [Route("checkout")]
        public IActionResult Checkout([FromBody] Payment payment)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);

            if (_orderService.CheckoutOrder(user, payment))
                return Ok("The order was successfully completed");
            else
                return BadRequest("Unable to complete this order");
        }

        [HttpPut("{id}/remove")]

        public IActionResult RemoveProductFromChart(int id)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            if (_orderService.RemoveFromChart(user, id))
                return Ok("Product was successfully removed from the chart");
            else
                return BadRequest("Unable to remove this item from the chart");
        }

        [Route("{id}/{ch}")]
        [HttpPut]
        public IActionResult ChangeProductQuantity(int id, string ch)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            if (_orderService.IncreaseItemQuantity(user, id))
                return Ok("The quantity of the product was successfully increased");
            else
                return BadRequest("Unable to increase the quantity of this product");
        }

        [Route("{id}/dec")]
        [HttpPut]
        public IActionResult DecreaseProductQuantity(int id, string ch)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            if (_orderService.DecreaseItemQuantity(user, id))
                return Ok("The quantity of the product was successfully decreased");
            else
                return BadRequest("Unable to decrease the quantity of this product");
        }


    }
}