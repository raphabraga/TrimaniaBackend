using System.Security.Claims;
using System.Linq;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Interfaces.Services;
using System.Collections.Generic;
using Backend.Models.Exceptions;
using Backend.Models.Enums;
using Backend.Utils;
using Backend.ViewModels;
using Backend.Dtos;

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
        public OrderController(IOrderService oService, IUserService uService)
        {
            _orderService = oService;
            _userService = uService;
        }

        [HttpGet("{id}")]
        public IActionResult OrderById(int id)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            string role = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role).Value;
            User requestingUser = _userService.GetUserByLogin(login);
            Order order = _orderService.GetOrderById(requestingUser, id);
            if (order == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.OrderIdNotFound));
            return Ok(new ViewOrder(order));
        }

        [HttpGet]
        public IActionResult OrdersByUser([FromQuery(Name = "sort")] string sort, [FromQuery(Name = "page")] int? page)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            if (user == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.CredentialsNotFound));
            return Ok(_orderService.GetOrders(user, sort, page).Select(order => new ViewOrder(order)));
        }

        [HttpGet]
        [Route("open")]
        public IActionResult OpenOrder()
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            if (user == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.CredentialsNotFound));
            Order order = _orderService.GetOpenOrder(user);
            if (order == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.NoOpenOrders));
            return Ok(new ViewOrder(order));
        }

        [HttpGet]
        [Route("in-progress")]
        public IActionResult InProgressOrder()
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            if (user == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.CredentialsNotFound));
            List<Order> orders = _orderService.GetInProgressOrders(user);
            if (orders == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.NoInProgressOrders));
            return Ok(orders.Select(order => new ViewOrder(order)));
        }

        [HttpPost]
        public IActionResult AddToChart([FromBody] AddToChartRequest request)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            return Ok(new ViewItem(_orderService.AddToChart(user, request)));
        }

        [HttpPut]
        [Route("cancel")]
        public IActionResult CancelOrder()
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            return Ok(new ViewOrder(_orderService.CancelOrder(user)));
        }

        [HttpPut]
        [Route("checkout")]
        public IActionResult CheckoutOrder([FromBody] PaymentRequest payment)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            return Ok(new ViewOrder(_orderService.CheckoutOrder(user, payment)));
        }

        [HttpPut("remove-item/{id}")]
        public IActionResult RemoveFromChart(int id)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            return Ok(new ViewItem(_orderService.RemoveFromChart(user, id)));
        }

        [Route("increase-item/{id}")]
        [HttpPut]
        public IActionResult IncreaseQuantity(int id)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            return Ok(new ViewItem(_orderService.ChangeItemQuantity(user, id, "Increase")));
        }

        [Route("decrease-item/{id}")]
        [HttpPut]
        public IActionResult DecreaseQuantity(int id)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = _userService.GetUserByLogin(login);
            return Ok(new ViewItem(_orderService.ChangeItemQuantity(user, id, "decrease")));
        }
    }
}