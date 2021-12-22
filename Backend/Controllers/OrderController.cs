using System;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Linq;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models.ViewModels;
using Backend.Interfaces;

namespace Backend.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/api/v{version:apiVersion}/orders")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;

        public OrderController(IOrderService oService, IUserService uService, IProductService pService)
        {
            _orderService = oService;
            _userService = uService;
            _productService = pService;
        }

        [HttpGet("{id}")]
        public IActionResult OrderById(int id)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            string role = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role).Value;
            User user = _userService.GetUserByLogin(login);
            Order order = _orderService.GetOrderById(id);
            if (order == null)
                return NotFound("No order registered on the database with this ID.");
            if (role != "Administrator" && order.Client.Id != user.Id)
                return Unauthorized("Credentials not allowed for the operation.");
            return Ok(new ViewOrder(order));
        }

        [HttpGet]
        public IActionResult UserOrders()
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            if (user == null)
                return NotFound("No user registered on the database with this credentials.");
            return Ok(_orderService.GetOrders(user).Select(order => new ViewOrder(order)));
        }

        [HttpGet]
        [Route("open")]
        public IActionResult OpenOrder()
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            if (user == null)
                return NotFound("No user registered on the database with this credentials.");
            Order order = _orderService.GetOpenOrder(user);
            if (order == null)
                return NotFound("The user has no open orders.");
            return Ok(new ViewOrder(order));
        }

        [HttpPost]
        public IActionResult AddProductToChart([FromBody] AddToChartRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("JSON object provided is formatted wrong.");
            if (_productService.GetProductById(request.ProductId) == null)
                return NotFound("Product not registered on the database with this ID.");
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            Order order = _orderService.GetOpenOrder(user);
            if (order == null)
                order = _orderService.CreateOrder(user);
            ChartItem item = _orderService.AddToChart(order, request.ProductId, request.Quantity);
            if (item == null)
                return UnprocessableEntity("The quantity ordered exceed the number of the product in stock.");
            return Ok(item);
        }

        [HttpPut]
        [Route("cancel")]
        public IActionResult Cancel()
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            Order order = _orderService.GetOpenOrder(user);
            if (order == null)
                return UnprocessableEntity("There is no open order to be cancelled.");
            if (_orderService.CancelOrder(order))
                return Ok("The order was successfully cancelled");
            else
                return BadRequest("Unable to cancel this order.");
        }

        [HttpPut]
        [Route("checkout")]
        public IActionResult Checkout([FromBody] Payment payment)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            Order order = _orderService.GetOpenOrder(user);
            if (order == null)
                return NotFound("There is no open order in order to checkout a purchase.");
            if (_orderService.CheckoutOrder(order, payment))
                return Ok("The order was successfully completed");
            else
                return UnprocessableEntity("The chart is empty. Unable to checkout a purchase.");
        }

        [HttpPut("remove-item/{id}")]
        public IActionResult RemoveProductFromChart(int id)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            Order order = _orderService.GetOpenOrder(user);
            if (order == null)
                return NotFound("There is no open order in order to remove items from chart.");
            if (_orderService.RemoveFromChart(order, id))
                return Ok("Product was successfully removed from the chart");
            else
                return UnprocessableEntity("This item doesn't exist in the cart. Unable to remove it");
        }

        [Route("increase-item/{id}")]
        [HttpPut]
        public IActionResult IncreaseItemQuantity(int id)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            Order order = _orderService.GetOpenOrder(user);
            if (order == null)
                return NotFound("There is no open order in order to modify items from chart.");
            if (_orderService.ChangeItemQuantity(order, id, "Increase"))
                return Ok("The quantity of the product was successfully changed");
            else
                return UnprocessableEntity("This item doesn't exist in the cart. Unable to modify it");
        }

        [Route("decrease-item/{id}")]
        [HttpPut]
        public IActionResult DecreaseItemQuantity(int id)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            Order order = _orderService.GetOpenOrder(user);
            if (order == null)
                return NotFound("There is no open order in order to modify items from chart.");
            if (_orderService.ChangeItemQuantity(order, id, "Decrease"))
                return Ok("The quantity of the product was successfully changed");
            else
                return UnprocessableEntity("This item doesn't exist in the cart. Unable to modify it");
        }
    }
}