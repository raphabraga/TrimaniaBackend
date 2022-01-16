using System;
using System.Security.Claims;
using System.Linq;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models.ViewModels;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Backend.Models.Exceptions;
using Backend.Models.Enums;
using Backend.Utils;

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
            try
            {
                User requestingUser = _userService.GetUserByLogin(login);
                Order order = _orderService.GetOrderById(requestingUser, id);
                if (order == null)
                    throw new RegisterNotFoundException(ErrorMessage.GetMessage(ErrorType.OrderIdNotFound));
                return Ok(new ViewOrder(order));
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (RegisterNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
                return NotFound(e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                System.Console.WriteLine(e.Message);
                return Unauthorized(e.Message);
            }
        }

        [HttpGet]
        public IActionResult OrdersByUser([FromQuery(Name = "sort")] string sort, [FromQuery(Name = "page")] int? page)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            try
            {
                User user = _userService.GetUserByLogin(login);
                if (user == null)
                    throw new RegisterNotFoundException(ErrorMessage.GetMessage(ErrorType.CredentialsNotFound));
                return Ok(_orderService.GetOrders(user, sort, page).Select(order => new ViewOrder(order)));
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (RegisterNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
                return NotFound(e.Message);
            }
        }

        [HttpGet]
        [Route("open")]
        public IActionResult OpenOrder()
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            try
            {
                User user = _userService.GetUserByLogin(login);
                if (user == null)
                    throw new RegisterNotFoundException(ErrorMessage.GetMessage(ErrorType.CredentialsNotFound));
                Order order = _orderService.GetOpenOrder(user);
                if (order == null)
                    throw new RegisterNotFoundException(ErrorMessage.GetMessage(ErrorType.NoOpenOrders));
                return Ok(new ViewOrder(order));
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (RegisterNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
                return NotFound(e.Message);
            }
        }

        [HttpGet]
        [Route("in-progress")]
        public IActionResult InProgressOrder()
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            try
            {
                User user = _userService.GetUserByLogin(login);
                if (user == null)
                    throw new RegisterNotFoundException(ErrorMessage.GetMessage(ErrorType.CredentialsNotFound));
                List<Order> orders = _orderService.GetInProgressOrders(user);
                if (orders == null)
                    throw new RegisterNotFoundException(ErrorMessage.GetMessage(ErrorType.NoInProgressOrders));
                return Ok(orders.Select(order => new ViewOrder(order)));
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (RegisterNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
                return NotFound(e.Message);
            }
        }

        [HttpPost]
        public IActionResult AddToChart([FromBody] AddToChartRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            try
            {
                User user = _userService.GetUserByLogin(login);
                return Ok(_orderService.AddToChart(user, request));
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (OutOfStockException e)
            {
                System.Console.WriteLine(e.Message);
                return UnprocessableEntity(e.Message);
            }
            catch (RegisterNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
                return NotFound(e.Message);
            }
        }

        [HttpPut]
        [Route("cancel")]
        public IActionResult CancelOrder()
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            try
            {
                User user = _userService.GetUserByLogin(login);
                return Ok(new ViewOrder(_orderService.CancelOrder(user)));
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (RegisterNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
                return NotFound(e.Message);
            }
        }

        [HttpPut]
        [Route("checkout")]
        public IActionResult CheckoutOrder([FromBody] Payment payment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                User user = _userService.GetUserByLogin(login);
                return Ok(new ViewOrder(_orderService.CheckoutOrder(user, payment)));
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (RegisterNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
                return NotFound(e.Message);
            }
        }

        [HttpPut("remove-item/{id}")]
        public IActionResult RemoveFromChart(int id)
        {
            try
            {
                string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                User user = _userService.GetUserByLogin(login);
                return Ok(_orderService.RemoveFromChart(user, id));
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (RegisterNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
                return NotFound(e.Message);
            }
        }

        [Route("increase-item/{id}")]
        [HttpPut]
        public IActionResult IncreaseQuantity(int id)
        {
            try
            {
                string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                User user = _userService.GetUserByLogin(login);
                return Ok(_orderService.ChangeItemQuantity(user, id, "Increase"));
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (RegisterNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
                return NotFound(e.Message);
            }
            catch (OutOfStockException e)
            {
                System.Console.WriteLine(e.Message);
                return UnprocessableEntity(e.Message);
            }
        }

        [Route("decrease-item/{id}")]
        [HttpPut]
        public IActionResult DecreaseQuantity(int id)
        {
            try
            {
                string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                User user = _userService.GetUserByLogin(login);
                return Ok(_orderService.ChangeItemQuantity(user, id, "decrease"));
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (RegisterNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
                return NotFound(e.Message);
            }
            catch (OutOfStockException e)
            {
                System.Console.WriteLine(e.Message);
                return UnprocessableEntity(e.Message);
            }
        }
    }
}