using System;
using System.Security.Claims;
using System.Linq;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models.ViewModels;
using Backend.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Backend.Models.Exceptions;

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
                User user = _userService.GetUserByLogin(login);
                Order order = _orderService.GetOrderById(id);
                if (order == null)
                    throw new RegisterNotFoundException("No order registered on the database with this ID.");
                if (role != "Administrator" && order.Client.Id != user.Id)
                    throw new UnauthorizedAccessException("Credentials not allowed for the operation.");
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
        public IActionResult UserOrders([FromQuery(Name = "sort")] string sort, [FromQuery(Name = "page")] int? page)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            try
            {
                User user = _userService.GetUserByLogin(login);
                if (user == null)
                    throw new RegisterNotFoundException("No user registered on the database with this credentials.");
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
                    throw new RegisterNotFoundException("No user registered on the database with this credentials.");
                Order order = _orderService.GetOpenOrder(user);
                if (order == null)
                    throw new RegisterNotFoundException("The user has no open orders.");
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
                    throw new RegisterNotFoundException("No user registered on the database with this credentials.");
                List<Order> orders = _orderService.GetInProgressOrders(user);
                if (orders == null)
                    throw new RegisterNotFoundException("The user has no orders in progress.");
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
        public IActionResult AddProductToChart([FromBody] AddToChartRequest request)
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
        public IActionResult Cancel()
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
        public IActionResult Checkout([FromBody] Payment payment)
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
        public IActionResult RemoveProductFromChart(int id)
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
        public IActionResult IncreaseItemQuantity(int id)
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
        public IActionResult DecreaseItemQuantity(int id)
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