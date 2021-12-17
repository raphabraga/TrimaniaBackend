using System;
using System.Linq;
using Backend.Data;
using Backend.Models.ViewModels;

namespace Backend.Services
{
    public class SalesReportService
    {
        private readonly ApplicationContext _applicationContext;

        public SalesReportService(ApplicationContext context)
        {
            _applicationContext = context;
        }
        public SalesReport GenerateReport(DateTime? startDate, DateTime? endDate,
        string userFilter, string statusFilter)
        {
            _applicationContext.Orders.Select(order => order.CreationDate > startDate && order.CreationDate < endDate);
            return new SalesReport();
        }
    }
}