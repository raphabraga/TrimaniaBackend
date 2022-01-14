using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Backend.Interfaces.Services;
using Backend.Models;
using Backend.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/orders-summary")]

    public class SalesReportController : ControllerBase
    {
        private readonly ISalesReportService _salesReportService;
        private readonly IUserService _userService;

        public SalesReportController(ISalesReportService rService, IUserService uService)
        {
            _salesReportService = rService;
            _userService = uService;
        }

        [HttpPost]
        public IActionResult SalesReport([FromBody] ReportFilter filter)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                string role = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role).Value;
                User user = _userService.GetUserByLogin(login);
                if (role != "Administrator")
                    filter.UserFilter = new List<string>() { login };
                return Ok(_salesReportService.GenerateReport(filter.StartDate,
                    filter.EndDate, filter.UserFilter, filter.StatusFilter));
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}