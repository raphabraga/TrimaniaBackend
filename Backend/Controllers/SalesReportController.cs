using Backend.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Authorize]
    [Route("sales")]
    public class SalesReportController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetReport([FromBody] ReportFilter filter)
        {


            return Ok();
        }
    }
}