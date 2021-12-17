using Backend.Models.ViewModels;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Authorize]
    [Route("sales")]

    public class SalesReportController : ControllerBase
    {
        private readonly SalesReportService _salesReportService;

        public SalesReportController(SalesReportService service)
        {
            _salesReportService = service;
        }

        [HttpGet]
        public IActionResult GetReport([FromBody] ReportFilter filter)
        {
            SalesReport sales = _salesReportService.GenerateReport(filter.StartDate,
            filter.EndDate, filter.UserFilter, filter.StatusFilter);
            return Ok(sales);
        }
    }
}