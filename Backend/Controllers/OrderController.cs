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
using System.Threading.Tasks;

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
        public OrderController(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> OrderById(int id)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            string role = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role).Value;
            User requestingUser = await _userService.GetUserByLogin(login);
            Order order = await _orderService.GetOrderById(requestingUser, id);
            if (order == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.OrderIdNotFound));
            return Ok(new ViewOrder(order));
        }

        [HttpGet]
        public async Task<IActionResult> OrdersByUser([FromQuery(Name = "sort")] string sort, [FromQuery(Name = "page")] int? page)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = await _userService.GetUserByLogin(login);
            if (user == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.CredentialsNotFound));
            var userOrders = await _orderService.GetOrders(user, sort, page);
            return Ok(userOrders.Select(order => new ViewOrder(order)));
        }

        [HttpGet]
        [Route("open")]
        public async Task<IActionResult> OpenOrder()
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = await _userService.GetUserByLogin(login);
            if (user == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.CredentialsNotFound));
            Order order = await _orderService.GetOpenOrder(user);
            if (order == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.NoOpenOrders));
            return Ok(new ViewOrder(order));
        }

        [HttpGet]
        [Route("in-progress")]
        public async Task<IActionResult> InProgressOrder()
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = await _userService.GetUserByLogin(login);
            if (user == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.CredentialsNotFound));
            List<Order> orders = await _orderService.GetInProgressOrders(user);
            if (orders == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.NoInProgressOrders));
            return Ok(orders.Select(order => new ViewOrder(order)));
        }

        [HttpPost]
        public async Task<IActionResult> AddToChart([FromBody] AddToChartRequest request)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = await _userService.GetUserByLogin(login);
            return Ok(new ViewItem(await _orderService.AddToChart(user, request)));
        }

        [HttpPut]
        [Route("cancel")]
        public async Task<IActionResult> CancelOrder()
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = await _userService.GetUserByLogin(login);
            return Ok(new ViewOrder(await _orderService.CancelOrder(user)));
        }

        [HttpPut]
        [Route("checkout")]
        public async Task<IActionResult> CheckoutOrder([FromBody] PaymentRequest payment)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = await _userService.GetUserByLogin(login);
            return Ok(new ViewOrder(await _orderService.CheckoutOrder(user, payment)));
        }

        [HttpPut("remove-item/{id}")]
        public async Task<IActionResult> RemoveFromChart(int id)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = await _userService.GetUserByLogin(login);
            return Ok(new ViewItem(await _orderService.RemoveFromChart(user, id)));
        }

        [Route("increase-item/{id}")]
        [HttpPut]
        public async Task<IActionResult> IncreaseQuantity(int id)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = await _userService.GetUserByLogin(login);
            return Ok(new ViewItem(await _orderService.ChangeItemQuantity(user, id, "Increase")));
        }

        [Route("decrease-item/{id}")]
        [HttpPut]
        public async Task<IActionResult> DecreaseQuantity(int id)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User user = await _userService.GetUserByLogin(login);
            return Ok(new ViewItem(await _orderService.ChangeItemQuantity(user, id, "decrease")));
        }
    }
}