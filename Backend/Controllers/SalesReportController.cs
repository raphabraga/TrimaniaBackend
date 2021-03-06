using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.Dtos;
using Backend.Interfaces.Services;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
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

        public SalesReportController(ISalesReportService saleReportService, IUserService userService)
        {
            _salesReportService = saleReportService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> SalesReport([FromBody] ReportRequest reportRequest)
        {
            string login = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            User requestingUser = await _userService.GetUserByLogin(login);
            return Ok(await _salesReportService.GenerateReport(requestingUser, reportRequest));
        }
    }
}