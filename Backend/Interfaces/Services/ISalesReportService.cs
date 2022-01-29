using Backend.ViewModels;
using Backend.Models;
using System.Threading.Tasks;
using Backend.Dtos;

namespace Backend.Interfaces.Services
{
    public interface ISalesReportService
    {
        public abstract Task<SalesReport> GenerateReport(User userRequesting, ReportRequest reportRequest);

    }
}