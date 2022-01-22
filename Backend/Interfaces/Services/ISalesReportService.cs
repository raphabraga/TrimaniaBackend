using System;
using System.Collections.Generic;
using Backend.ViewModels;
using Backend.Models;
using Backend.Models.Enums;
using System.Threading.Tasks;

namespace Backend.Interfaces.Services
{
    public interface ISalesReportService
    {
        public abstract Task<SalesReport> GenerateReport(User userRequesting, DateTime startDate, DateTime endDate,
        List<string> userFilter, List<OrderStatus> statusFilter);

    }
}