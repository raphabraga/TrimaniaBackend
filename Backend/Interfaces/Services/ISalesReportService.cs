using System;
using System.Collections.Generic;
using Backend.Models;
using Backend.Models.ViewModels;

namespace Backend.Interfaces.Services
{
    public interface ISalesReportService
    {
        public abstract SalesReport GenerateReport(DateTime startDate, DateTime endDate,
        List<string> userFilter, List<OrderStatus> statusFilter);

    }
}